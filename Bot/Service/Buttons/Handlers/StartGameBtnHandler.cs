using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class StartGameBtnHandler(
        ITelegramBotClient bot,
        IDateTimeProvider dateTimeProvider,
        BotContext botContext,
        UserRepository userRepository,
        GameKeyboardFactory keyboardBuilder,
        ScoreCalculator scoreCalculator) : IButtonsHandler
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

            //
            var callbackQueryMsg = string.Empty;
            var textMessage = string.Empty;
            var keyboard = new InlineKeyboardMarkup();

            // Creating game
            var game = new Game(player1, player2, dateTimeProvider.UtcNow);
            await botContext.Games.AddAsync(game);
            await botContext.SaveChangesAsync();

            // Creating turn, roll dice and save it
            game.StartTurn(player1);
            game.CurrentTurn!.RollDice();

            await botContext.SaveChangesAsync();

            if (scoreCalculator.IsFarkle(game.CurrentTurn!.DiceValue))
            {
                callbackQueryMsg = "Невдача :с";
                textMessage = $"Ви програли!";
                keyboard = InlineKeyboardMarkup.Empty();
            }
            else
            {
                // Creating buttons
                keyboard = keyboardBuilder.BuildDiceSelectionKeyboard(game.CurrentTurn);

                // Creating bot response
                callbackQueryMsg = $"Ви прийняли виклик{player1.FirstName}";
                textMessage = $"@{player2.UserName}(p2) прийняв виклик @{player1.UserName}(p1).\n @{player1.UserName}(p1) ващ хід:";
            }
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