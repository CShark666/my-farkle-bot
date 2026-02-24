using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Bot
{
    public class SaveAndRollBtnHandler(
        ITelegramBotClient bot,
        BotContext botContext,
        GameKeyboardFactory keyboardBuilder,
        GameCallbackDataSerializer callbackDataSerializer,
        ScoreCalculator scoreCalculator) : IButtonsHandler
    {
        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            // Getting game data
            callbackDataSerializer.Deserialize(query.Data!, out var gameId);
            var game = await botContext.Games
                        .Include(g => g.CurrentTurn)
                        .Include(g => g.CurrentTurn!.Player)
                        .FirstOrDefaultAsync(g => g.Id == gameId);

            var turn = game!.CurrentTurn;
            var currentScore = turn!.CurrentScore;

            // Save score and roll remaining dice 
            turn.SaveAdnRoll();
            await botContext.SaveChangesAsync();

            // Creating buttons
            var diceKeyboard = keyboardBuilder.BuildDiceSelectionKeyboard(turn);
            var saveAndRollButton = keyboardBuilder.SaveAndRollButton(turn);
            var saveAndEndButton = keyboardBuilder.SaveAndEndButton(turn);

            diceKeyboard.AddNewRow(saveAndRollButton);
            diceKeyboard.AddNewRow(saveAndEndButton);

            // Creating bot response
            var callbackQueryMsg = $"Супер! Ви отримали {currentScore} очок!";
            var textMessage = $"🎲 Хід @{turn.Player.UserName} (p1)\nРахунок за раунд: {turn.TotalScore}\nРахунок вибраних кубиків: {turn.CurrentScore}";

            try
            {
                await bot.EditMessageText(
                    chatId: turn.Player.ChatId,
                    messageId: query.Message!.Id,
                    text: textMessage,
                    replyMarkup: diceKeyboard);

                await bot.AnswerCallbackQuery(query.Id, callbackQueryMsg);
            }
            catch (ApiRequestException ex)
                when (ex.Message.Contains("message is not modified"))
            {

            }
        }
    }
}