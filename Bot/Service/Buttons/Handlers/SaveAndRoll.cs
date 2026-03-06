using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot
{
    public class SaveAndRollBtnHandler(
        TelegramBotClient bot,
        BotContext botContext,
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
            callbackDataSerializer.Deserialize(query.Data!, out var gameId);
            var validationResult = await validator.ValidateDiceSelectionAsync(callbackData.UserId, query.From.Id, gameId);

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

                validationResult = validator.ValidateFarkle(turn.DiceValue, game);
                if (!validationResult.IsValid)
                {
                    response = validationResult.Response!;
                }
                else
                {
                    response = responseFactory.BuildSaveAndRollResponse(game.CurrentTurn!);
                }
            }

            await bot.SafeEditAndAnswerAsync(
                    callbackData.ChatId, query.Message!.Id, query.Id, response);
        }
    }
}