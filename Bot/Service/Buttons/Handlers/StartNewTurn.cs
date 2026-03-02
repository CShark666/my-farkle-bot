using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class StartNewTurnBtnHandler(
            TelegramBotClient bot,
            BotContext botContext,
            GameButtonsBuilder keyboardBuilder,
            GameCallbackDataSerializer callbackDataSerializer,
            GameResponseFactory messageBuilder,
            ScoreCalculator scoreCalculator,
            GameRepository repository) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.StartTurn;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            var keyboard = new InlineKeyboardMarkup();
            callbackDataSerializer.Deserialize(query.Data!, out var gameId);

            if (callbackData.MatchesId(query.From.Id))
            {
                response = messageBuilder.BuildWrongTurnResponse();
            }
            else if (await repository.IsGameFinishedAsync(gameId))
            {
                response = messageBuilder.BuildGameIsFinished();
            }

            else
            {
                var game = await repository.GetGameAsync(gameId);

                var player = game!.GetOpponent();

                game.StartTurn(player);
                game.CurrentTurn!.RollDice();

                await botContext.SaveChangesAsync();


                if (scoreCalculator.IsFarkle(game.CurrentTurn!.DiceValue))
                {
                    response = messageBuilder.BuildFarkleResponse(game);
                    keyboard = InlineKeyboardMarkup.Empty();
                }
                else
                {
                    keyboard = keyboardBuilder.BuildTurnKeyboard(game.CurrentTurn);
                    response = messageBuilder.BuildStartTurnResponse(game);
                }

                response = messageBuilder.BuildSaveAndRollResponse(game.CurrentTurn);
                keyboard = keyboardBuilder.BuildTurnKeyboard(game.CurrentTurn);
            }
            await bot.SafeEditAndAnswerAsync(
                callbackData.ChatId, query.Message!.Id, response.Text,
                keyboard, query.Id, response.QueryMessage);
        }
    }
}