using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class StartGameBtnHandler(
        TelegramBotClient bot,
        BotContext botContext,
        UserRepository userRepository,
        GameButtonsFactory keyboardFactory,
        ScoreCalculator scoreCalculator,
        GameResponseFactory responseFactory) : IButtonsHandler
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
                response = responseFactory.BuildWrongPlayerResponse();
            }
            else if(!await userRepository.IsUsersStatusValid(player1, player2))
            {
                response = responseFactory.BuildInvalidUserGamesStatus();
            }

            else
            {
                var game = new Game(player1, player2);

                await botContext.Games.AddAsync(game);
                await botContext.SaveChangesAsync();


                game.StartTurn(player1);
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