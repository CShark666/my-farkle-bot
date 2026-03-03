using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class SaveAndEndBtnHandler(
            TelegramBotClient bot,
            BotContext botContext,
            GameButtonsFactory keyboardFactory,
            GameCallbackDataSerializer callbackDataSerializer,
            GameResponseFactory responseFactory,
            GameRepository repository,
            ValidatorService validator) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.SaveAndEnd;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            var keyboard = new InlineKeyboardMarkup();
            callbackDataSerializer.Deserialize(query.Data!, out var gameId);

            var validationResult = await validator.ValidationAsync([
                new UserIdValidator(callbackData.UserId, query.From.Id, responseFactory),
                new GameStatusValidator(gameId, botContext, responseFactory)
            ]);
            if (!validationResult.IsValid)
            {
                response = validationResult.Response!;
            }
            else
            {
                var game = await repository.GetGameAsync(gameId);

                game!.FinishTurn();

                if (game.IsPlayerWins())
                {
                    game.FinishGame();
                    response = responseFactory.BuildFinishGameResponse(game);
                }
                else
                {
                    keyboard = keyboardFactory.BuildEndTurnKeyboard(game);
                    response = responseFactory.BuildSaveAndEndResponse(game);
                }

                await botContext.SaveChangesAsync();
            }

            await bot.SafeEditAndAnswerAsync(
                callbackData.ChatId, query.Message!.Id, response.Text,
                keyboard, query.Id, response.QueryMessage);
        }
    }
}