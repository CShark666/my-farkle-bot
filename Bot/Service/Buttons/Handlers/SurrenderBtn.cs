using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot
{
    public class SurrenderBtnHandler(
            TelegramBotClient bot,
            BotContext botContext,
            GameCallbackDataSerializer callbackDataSerializer,
            GameResponseFactory responseFactory,
            GameRepository repository,
            ValidatorService validator) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.Surrender;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            callbackDataSerializer.Deserialize(query.Data!, out var gameId);
            var validationResult = await validator.ValidateUsersAndGameAsync(gameId, query.From.Id);

            if (!validationResult.IsValid)
            {
                response = validationResult.Response!;
            }
            else
            {
                var game = await repository.GetGameAsync(gameId);

                game!.FinishGame();
                game.Winner = game.GetOpponentWithId(query.From.Id);

                await botContext.SaveChangesAsync();

                response = responseFactory.BuildSurrenderResponse(game);
            }


            await bot.SafeEditAndAnswerAsync(
                callbackData.ChatId, query.Message!.Id, query.Id, response);
        }
    }
}