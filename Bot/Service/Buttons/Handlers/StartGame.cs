using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot
{
    public class StartGameBtnHandler(
        TelegramBotClient bot,
        BotContext botContext,
        UserRepository userRepository,
        ScoreCalculator scoreCalculator,
        GameResponseFactory responseFactory,
        ValidatorService validator) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.StartGame;

        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
        {
            BotResponse response;
            
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

            var validationResult = await validator.ValidateOpponentsAsync(player1, player2);

            if (!validationResult.IsValid)
            {
                response = validationResult.Response!;
            }
            else
            {
                var game = new Game(player1, player2);

                await botContext.Games.AddAsync(game);
                await botContext.SaveChangesAsync();


                game.StartTurn(player1);
                game.CurrentTurn!.RollDice();

                await botContext.SaveChangesAsync();


                validationResult = validator.ValidateFarkle(game.CurrentTurn.DiceValue, game);
                if (!validationResult.IsValid)
                {
                    response = validationResult.Response!;
                }
                else
                {
                    response = responseFactory.BuildStartTurnResponse(game);
                }
            }

            await bot.SafeEditAndAnswerAsync(
                    callbackData.ChatId, query.Message!.Id, query.Id, response);
        }
    }
}