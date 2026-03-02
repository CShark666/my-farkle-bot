using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class SelectDiceBtnHandler(
        TelegramBotClient bot,
        BotContext botContext,
        GameButtonsBuilder keyboardBuilder,
        GameCallbackDataSerializer callbackDataSerializer,
        ScoreCalculator scoreCalculator,
        GameResponseFactory messageBuilder,
        GameRepository repository) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.SelectDice;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            var keyboard = InlineKeyboardMarkup.Empty();
            callbackDataSerializer.Deserialize(query.Data!, out var gameId, out var selectedDiceId);

            if (!callbackData.MatchesId(query.From.Id))
            {
                response = messageBuilder.BuildWrongTurnResponse();
            }
            else if (await repository.IsGameFinishedAsync(gameId))
            {
                response = messageBuilder.BuildGameIsFinished();
            }

            else
            {
                var game = await repository.GetGameTurnAsync(gameId);

                var turn = game!.CurrentTurn;
                turn!.AddOrRemoveDiceSelection(selectedDiceId);


                var selectedDice = turn.SelectedDice.Select(sd => turn.DiceValue[sd]).ToArray();
                turn.CurrentScore = scoreCalculator.Calculate(selectedDice);


                await botContext.SaveChangesAsync();

                keyboard = keyboardBuilder.BuildTurnKeyboard(turn);
                response = messageBuilder.BuildSelectDiceResponse(turn, selectedDiceId);
            }

            await bot.SafeEditAndAnswerAsync(
                    callbackData.ChatId, query.Message!.Id, response.Text,
                    keyboard, query.Id, response.QueryMessage);
        }
    }
}