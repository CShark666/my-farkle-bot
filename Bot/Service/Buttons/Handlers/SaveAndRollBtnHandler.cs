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
        GameButtonsBuilder keyboardBuilder,
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
            var queryMsg = string.Empty;
            var textMsg = string.Empty;
            var keyboard = new InlineKeyboardMarkup();
            var turn = game!.CurrentTurn;

            // Save score and roll remaining dice 
            turn!.SaveAndRoll();
            await botContext.SaveChangesAsync();

            if (scoreCalculator.IsFarkle(turn.DiceValue))
            {
                queryMsg = "Невдача :с";
                textMsg = $"Ви програли! Та втратили {turn.TotalScore}";
                keyboard = InlineKeyboardMarkup.Empty();
            }
            else
            {
                // Creating buttons
                keyboard = keyboardBuilder.BuildTurnKeyboard(turn);

                // Creating bot response
                queryMsg = $"Супер! Ви отримали {turn.CurrentScore} очок!";
                textMsg = $"🎲 Хід @{turn.Player.UserName} (p1)\nРахунок за раунд: {turn.TotalScore}\nРахунок вибраних кубиків: {turn.CurrentScore}";
            }

            await bot.SafeEditAndAnswerAsync(
                callbackData.ChatId, query.Message!.Id, textMsg,
                keyboard, query.Id, queryMsg);
        }
    }
}