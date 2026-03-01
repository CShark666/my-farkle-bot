using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class StartGameBtnHandler(
        TelegramBotClient bot,
        IDateTimeProvider dateTimeProvider,
        BotContext botContext,
        UserRepository userRepository,
        GameButtonsBuilder keyboardBuilder,
        ScoreCalculator scoreCalculator,
        GameMessageBuilder messageBuilder) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.StartGame;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            var keyboard = InlineKeyboardMarkup.Empty();

            if (callbackData.MatchesId(query.From.Id))
            {
                response = messageBuilder.BuildWrongPlayerResponse();
            }
            else
            {
                // Creating players
                var player1 = await userRepository.GetOrCreateUserAsync(
                                            new User(
                                                callbackData.ChatId,
                                                callbackData.UserId));

                var player2 = await userRepository.GetOrCreateUserAsync(
                                            new User(
                                                callbackData.ChatId,
                                                query.From.Id,
                                                query.From.Username!,
                                                query.From.FirstName));

                // Creating game
                var game = new Game(player1, player2, dateTimeProvider.UtcNow);
                await botContext.Games.AddAsync(game);
                await botContext.SaveChangesAsync();

                // Creating turn, roll dice and save it
                game.StartTurn(player1);
                game.CurrentTurn!.RollDice();

                await botContext.SaveChangesAsync();


                // Farkle check
                if (scoreCalculator.IsFarkle(game.CurrentTurn!.DiceValue))
                {
                    response = messageBuilder.BuildFarkleResponse(game);
                    keyboard = InlineKeyboardMarkup.Empty();
                }
                else
                {
                    keyboard = keyboardBuilder.BuildTurnKeyboard(game.CurrentTurn);
                    response = messageBuilder.BuildStartTurnResponse(game);
                }
            }

            await bot.SafeEditAndAnswerAsync(
                    callbackData.ChatId, query.Message!.Id, response.Text,
                    keyboard, query.Id, response.QueryMessage);
        }
    }
}