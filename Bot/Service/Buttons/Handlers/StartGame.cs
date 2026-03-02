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
        GameResponseFactory messageBuilder) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.StartGame;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            var keyboard = InlineKeyboardMarkup.Empty();
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

            if (callbackData.MatchesId(query.From.Id))
            {
                response = messageBuilder.BuildWrongPlayerResponse();
            }
            else if(!await userRepository.IsUsersStatusValid(player1, player2))
            {
                response = messageBuilder.BuildInvalidUserGamesStatus();
            }

            else
            {
                var game = new Game(player1, player2, dateTimeProvider.UtcNow);

                await botContext.Games.AddAsync(game);
                await botContext.SaveChangesAsync();


                game.StartTurn(player1);
                game.CurrentTurn!.RollDice();

                await botContext.SaveChangesAsync();


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