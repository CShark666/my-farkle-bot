using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class SelectDiceBtnHandler(
        TelegramBotClient bot,
        BotContext botContext,
        GameButtonsFactory keyboardFactory,
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
            var keyboard = InlineKeyboardMarkup.Empty();
            callbackDataSerializer.Deserialize(query.Data!, out var gameId, out var selectedDiceId);

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
                var game = await repository.GetGameTurnAsync(gameId);
                var turn = game!.CurrentTurn;

                turn!.AddOrRemoveDiceSelection(selectedDiceId);

                var selectedDice = turn.SelectedDice.Select(sd => turn.DiceValue[sd]).ToArray();
                turn.CurrentScore = scoreCalculator.Calculate(selectedDice);

                await botContext.SaveChangesAsync();

                keyboard = keyboardFactory.BuildTurnKeyboard(turn);
                response = responseFactory.BuildSelectDiceResponse(turn, selectedDiceId);
            }

            await bot.SafeEditAndAnswerAsync(
                    callbackData.ChatId, query.Message!.Id, response.Text,
                    keyboard, query.Id, response.QueryMessage);
        }
    }
}