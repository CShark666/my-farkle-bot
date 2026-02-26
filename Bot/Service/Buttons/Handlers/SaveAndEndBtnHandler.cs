using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Bot
{
    public class SaveAndEndBtnHandler(
            TelegramBotClient bot,
            BotContext botContext,
            GameButtonsBuilder keyboardBuilder,
            GameCallbackDataSerializer callbackDataSerializer) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.SaveAndEnd;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            // Getting game data
            callbackDataSerializer.Deserialize(query.Data!, out var gameId);
            var game = await botContext.Games
                        .Include(g => g.CurrentTurn)
                        .Include(g => g.Player1)
                        .Include(g => g.Player2)
                        .FirstOrDefaultAsync(g => g.Id == gameId);

            var turn = game!.CurrentTurn;
            var currentScore = turn!.CurrentScore;

            // Save and end turn
            turn.TotalScore += turn.CurrentScore;
            turn.Player.TotalScore += turn.TotalScore;

            var newPlayer = game.GetOpponent();

            game.StartTurn(newPlayer);
            game.CurrentTurn!.RollDice();

            await botContext.SaveChangesAsync();

            // Creating buttons
            var keyboard = keyboardBuilder.BuildTurnKeyboard(game.CurrentTurn!);

            // Creating bot response
            var callbackQueryMsg = $" ";
            var textMessage = $"🎲 Хід @{turn.Player.UserName}\nРахунок за раунд: {turn.TotalScore}\nРахунок вибраних кубиків: {turn.CurrentScore}";

            await bot.SafeEditAndAnswerAsync(
                callbackData.ChatId, query.Message!.Id, textMessage,
                keyboard, query.Id, callbackQueryMsg);
        }
    }
}