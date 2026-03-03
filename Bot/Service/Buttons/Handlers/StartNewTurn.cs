using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class StartNewTurnBtnHandler(
            TelegramBotClient bot,
            BotContext botContext,
            GameButtonsFactory keyboardFactory,
            GameCallbackDataSerializer callbackDataSerializer,
            GameResponseFactory responseFactory,
            ScoreCalculator scoreCalculator,
            GameRepository repository,
            ValidatorService validator) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.StartTurn;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            var keyboard = new InlineKeyboardMarkup();
            callbackDataSerializer.Deserialize(query.Data!, out var gameId);

            var validationResult = await validator.ValidationAsync([
                new GameStatusValidator(gameId, botContext, responseFactory)
            ]);
            
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

                if (scoreCalculator.IsFarkle(game.CurrentTurn!.DiceValue))
                {
                    response = responseFactory.BuildFarkleResponse(game);
                    keyboard = InlineKeyboardMarkup.Empty();
                }
                else
                {
                    keyboard = keyboardFactory.BuildTurnKeyboard(game.CurrentTurn);
                    response = responseFactory.BuildStartTurnResponse(game);
                }
            }
            await bot.SafeEditAndAnswerAsync(
                callbackData.ChatId, query.Message!.Id, response.Text,
                keyboard, query.Id, response.QueryMessage);
        }
    }
}