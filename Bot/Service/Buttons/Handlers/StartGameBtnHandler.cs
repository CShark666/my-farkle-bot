using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace Bot
{
    public class StartGameBtnHandler(
        ITelegramBotClient bot,
        IDateTimeProvider dateTimeProvider,
        BotContext botContext,
        UserRepository userRepository,
        GameKeyboardFactory keyboardBuilder) : IButtonsHandler
    {
        public async Task HandleButton(CallbackData callbackData, CallbackQuery query)
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

            // Creating and save first turn + roll dice
            var firstTurn = new Turn(game.Id, player1);
            game.CurrentTurn = firstTurn;

            firstTurn.RollDice();

            await botContext.SaveChangesAsync();


            // Creating buttons
            var keyboard = keyboardBuilder.BuildDiceSelectionKeyboard(firstTurn);

            // Creating bot response
            var callbackQueryMsg = $"Ви прийняли виклик{player1.FirstName}";
            var textMessage = $"@{player2.UserName}(p2) прийняв виклик @{player1.UserName}(p1).\n @{player1.UserName}(p1) ващ хід:";
            try
            {
                await bot.EditMessageText(
                    chatId: player1.ChatId,
                    messageId: query.Message!.Id,
                    text: textMessage,
                    replyMarkup: keyboard);
                await bot.AnswerCallbackQuery(query.Id, callbackQueryMsg);
            }
            catch (ApiRequestException ex)
                when (ex.Message.Contains("message is not modified"))
            {

            }
        }
    }
}