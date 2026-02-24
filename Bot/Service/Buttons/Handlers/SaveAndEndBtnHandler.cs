using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Bot
{
    public class SaveAndEndBtnHandler(
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
                        .Include(g => g.Player1)
                        .Include(g => g.Player2)
                        .FirstOrDefaultAsync(g => g.Id == gameId);
            
            var turn = game!.CurrentTurn;
            var currentScore = turn!.CurrentScore;

            // Save and end turn
            turn.TotalScore += turn.CurrentScore;
            turn.Player.TotalScore += turn.TotalScore;

            var newPlayer = game.GetNewCurrentPlayer();
            var newTurn = new Turn(game.Id, newPlayer);
            game.CurrentTurn = newTurn;
            game.CurrentTurn.RollDice();

            await botContext.SaveChangesAsync();

            // Creating buttons
            var diceKeyboard = keyboardBuilder.BuildDiceSelectionKeyboard(turn);
            var saveAndRollButton = keyboardBuilder.SaveAndRollButton(turn);
            var saveAndEndButton = keyboardBuilder.SaveAndEndButton(turn);

            diceKeyboard.AddNewRow(saveAndRollButton);
            diceKeyboard.AddNewRow(saveAndEndButton);

            // Creating bot response
            var callbackQueryMsg = $" ";
            var textMessage = $"🎲 Хід @{turn.Player.UserName}\nРахунок за раунд: {turn.TotalScore}\nРахунок вибраних кубиків: {turn.CurrentScore}";

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