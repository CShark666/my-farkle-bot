using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class SaveAndRollBtnHandler(
        TelegramBotClient bot,
        BotContext botContext,
        GameButtonsBuilder keyboardBuilder,
        GameCallbackDataSerializer callbackDataSerializer,
        ScoreCalculator scoreCalculator,
        GameMessageBuilder messageBuilder,
        GameRepository repository) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.SaveAndRoll;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            var keyboard = new InlineKeyboardMarkup();

            if (!callbackData.MatchesId(query.From.Id))
            {
                response = messageBuilder.BuildWrongTurnResponse();
            }

            callbackDataSerializer.Deserialize(query.Data!, out var gameId);
            var gameIsFinished = await repository.IsGameFinishedAsync(gameId);

            if (gameIsFinished)
            {
                response = messageBuilder.BuildGameIsFinished();
            }
            
            else
            {
                var game = await repository.GetGameAsync(gameId);
                var turn = game!.CurrentTurn;

                turn!.SaveAndRoll();

                await botContext.SaveChangesAsync();

                if (scoreCalculator.IsFarkle(turn.DiceValue))
                {
                    response = messageBuilder.BuildFarkleResponse(game);
                    keyboard = keyboardBuilder.BuildEndTurnKeyboard(game);
                }
                else
                {
                    keyboard = keyboardBuilder.BuildTurnKeyboard(turn);
                    response = messageBuilder.BuildSaveAndRollResponse(game.CurrentTurn!);
                }
            }

            await bot.SafeEditAndAnswerAsync(
                    callbackData.ChatId, query.Message!.Id, response.Text,
                    keyboard, query.Id, response.QueryMessage);
        }
    }
}