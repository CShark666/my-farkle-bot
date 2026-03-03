using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class SaveAndRollBtnHandler(
        TelegramBotClient bot,
        BotContext botContext,
        GameButtonsFactory keyboardFactory,
        GameCallbackDataSerializer callbackDataSerializer,
        ScoreCalculator scoreCalculator,
        GameResponseFactory responseFactory,
        GameRepository repository,
        ValidatorService validator) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.SaveAndRoll;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            var keyboard = new InlineKeyboardMarkup();
            callbackDataSerializer.Deserialize(query.Data!, out var gameId);

            var validationResult = await validator.ValidationAsync([
                new UserIdValidator(callbackData.UserId, query.From.Id,responseFactory),
                new GameStatusValidator(gameId, botContext, responseFactory)
            ]);
            
            if (!validationResult.IsValid)
            {
                response = validationResult.Response!;
            }
            else
            {
                var game = await repository.GetGameAsync(gameId);
                var turn = game!.CurrentTurn;

                turn!.SaveAndRoll();

                await botContext.SaveChangesAsync();

                if (scoreCalculator.IsFarkle(turn.DiceValue))
                {
                    response = responseFactory.BuildFarkleResponse(game);
                    keyboard = keyboardFactory.BuildEndTurnKeyboard(game);
                }
                else
                {
                    keyboard = keyboardFactory.BuildTurnKeyboard(turn);
                    response = responseFactory.BuildSaveAndRollResponse(game.CurrentTurn!);
                }
            }

            await bot.SafeEditAndAnswerAsync(
                    callbackData.ChatId, query.Message!.Id, response.Text,
                    keyboard, query.Id, response.QueryMessage);
        }
    }
}