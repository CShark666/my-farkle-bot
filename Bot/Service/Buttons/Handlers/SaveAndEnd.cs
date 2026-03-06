using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot
{
    public class SaveAndEndBtnHandler(
            TelegramBotClient bot,
            BotContext botContext,
            GameCallbackDataSerializer callbackDataSerializer,
            GameResponseFactory responseFactory,
            GameRepository repository,
            ValidatorService validator) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.SaveAndEnd;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            callbackDataSerializer.Deserialize(query.Data!, out var gameId);
            var validationResult = await validator.ValidateUserAndGame(callbackData.UserId, query.From.Id, gameId);

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
                    response = responseFactory.BuildSaveAndEndResponse(game);
                }

                await botContext.SaveChangesAsync();
            }

            await bot.SafeEditAndAnswerAsync(
                callbackData.ChatId, query.Message!.Id, query.Id, response);
        }
    }
}