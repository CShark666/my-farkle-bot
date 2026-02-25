using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class SaveAndRollBtnHandler(
        TelegramBotClient bot,
        BotContext botContext,
        GameKeyboardFactory keyboardBuilder,
        GameCallbackDataSerializer callbackDataSerializer,
        ScoreCalculator scoreCalculator) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.SaveAndRoll;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            // Getting game data
            callbackDataSerializer.Deserialize(query.Data!, out var gameId);
            var game = await botContext.Games
                        .Include(g => g.CurrentTurn)
                        .Include(g => g.CurrentTurn!.Player)
                        .FirstOrDefaultAsync(g => g.Id == gameId);


            // Creating variables
            var callbackQueryMsg = string.Empty;
            var textMessage = string.Empty;
            var keyboard = new InlineKeyboardMarkup();
            var turn = game!.CurrentTurn;
            var currentScore = turn!.CurrentScore;

            // Save score and roll remaining dice 
            turn.SaveAdnRoll();
            await botContext.SaveChangesAsync();

            if (scoreCalculator.IsFarkle(turn.DiceValue))
            {
                callbackQueryMsg = "Невдача :с";
                textMessage = $"Ви програли! Та втратили {currentScore}";
                keyboard = InlineKeyboardMarkup.Empty();
            }
            else
            {
                // Creating buttons
                keyboard = keyboardBuilder.BuildDiceSelectionKeyboard(turn);
                var saveAndRollButton = keyboardBuilder.SaveAndRollButton(turn);
                var saveAndEndButton = keyboardBuilder.SaveAndEndButton(turn);

                keyboard.AddNewRow(saveAndRollButton);
                keyboard.AddNewRow(saveAndEndButton);

                // Creating bot response
                callbackQueryMsg = $"Супер! Ви отримали {currentScore} очок!";
                textMessage = $"🎲 Хід @{turn.Player.UserName} (p1)\nРахунок за раунд: {turn.TotalScore}\nРахунок вибраних кубиків: {turn.CurrentScore}";
            }

            try
            {
                await bot.EditMessageText(
                    chatId: turn.Player.ChatId,
                    messageId: query.Message!.Id,
                    text: textMessage,
                    replyMarkup: keyboard);

                await bot.AnswerCallbackQuery(query.Id, callbackQueryMsg);
            }
            catch (ApiRequestException ex)
                when (ex.Message.Contains("message is not modified"))
            {

            }
        }
    }
}