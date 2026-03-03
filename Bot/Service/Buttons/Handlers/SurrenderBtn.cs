using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

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
            var keyboard = new InlineKeyboardMarkup();

            callbackDataSerializer.Deserialize(query.Data!, out var gameId);

            var validationResult = await validator.ValidationAsync([
                new GamePlayersIdsValidator(gameId, query.From.Id, botContext, responseFactory),
                new GameStatusValidator(gameId, botContext, responseFactory)
            ]);

            if(!validationResult.IsValid)
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
                callbackData.ChatId, query.Message!.Id, response.Text,
                keyboard, query.Id, response.QueryMessage);
        }
    }
}