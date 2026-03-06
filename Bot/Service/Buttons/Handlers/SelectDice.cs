using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot
{
    public class SelectDiceBtnHandler(
        TelegramBotClient bot,
        BotContext botContext,
        GameCallbackDataSerializer callbackDataSerializer,
        ScoreCalculator scoreCalculator,
        GameResponseFactory responseFactory,
        GameRepository repository,
        ValidatorService validator) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.SelectDice;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            callbackDataSerializer.Deserialize(query.Data!, out var gameId, out var selectedDiceId);
            var validationResult = await validator.ValidateUserAndGame(callbackData.UserId, query.From.Id, gameId);

            if (!validationResult.IsValid)
            {
                response = validationResult.Response!;
            }
            else
            {
                var game = await repository.GetGameTurnAsync(gameId);
                var turn = game!.CurrentTurn;

                turn!.AddOrRemoveDiceSelection(selectedDiceId);

                var selectedDice = turn.SelectedDice.Select(sd => turn.DiceValue[sd]).ToArray();
                turn.CurrentScore = scoreCalculator.Calculate(selectedDice);

                await botContext.SaveChangesAsync();

                response = responseFactory.BuildSelectDiceResponse(turn, selectedDiceId);
            }

            await bot.SafeEditAndAnswerAsync(
                    callbackData.ChatId, query.Message!.Id, query.Id, response);
        }
    }
}