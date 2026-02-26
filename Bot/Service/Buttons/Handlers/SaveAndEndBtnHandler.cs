using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class SaveAndEndBtnHandler(
            TelegramBotClient bot,
            BotContext botContext,
            GameButtonsBuilder keyboardBuilder,
            GameCallbackDataSerializer callbackDataSerializer,
            GameMessageBuilder messageBuilder) : IButtonsHandler
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
            // var keyboard = keyboardBuilder.BuildTurnKeyboard(game.CurrentTurn!);
            var keyboard = InlineKeyboardMarkup.Empty();
            
            // Creating bot response
            BotResponse response = messageBuilder.BuildSaveAndEndResponse(turn);

            await bot.SafeEditAndAnswerAsync(
                callbackData.ChatId, query.Message!.Id, response.Text,
                keyboard, query.Id, response.QueryMessage);
        }
    }
}