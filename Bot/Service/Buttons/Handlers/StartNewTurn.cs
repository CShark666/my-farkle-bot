using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot
{
    public class StartNewTurnBtnHandler(
            TelegramBotClient bot,
            BotContext botContext,
            GameCallbackDataSerializer callbackDataSerializer,
            GameResponseFactory responseFactory,
            GameRepository repository,
            ValidatorService validator) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.StartTurn;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse? response;
            callbackDataSerializer.Deserialize(query.Data!, out var gameId);
            var validationResult = await validator.ValidateGameStatus(gameId);

            if (!validationResult.IsValid)
            {
                response = validationResult.Response!;
            }
            else
            {
                var game = await repository.GetGameAsync(gameId);
                var player = game!.GetOpponent();

                game.StartTurn(player);
                game.CurrentTurn!.RollDice();

                await botContext.SaveChangesAsync();

                var farkleValidation = validator.ValidateFarkle(game.CurrentTurn.DiceValue, game);

                response = !farkleValidation.IsValid 
                ? farkleValidation.Response 
                : responseFactory.BuildStartTurnResponse(game);
            }
            await bot.SafeEditAndAnswerAsync(
                callbackData.ChatId, query.Message!.Id, query.Id, response!);
        }
    }
}