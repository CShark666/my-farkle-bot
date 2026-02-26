using Telegram.Bot;
using Telegram.Bot.Exceptions;
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
        ScoreCalculator scoreCalculator) : IButtonsHandler
    {
        public CallbackActionType Key => CallbackActionType.StartGame;

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
            var queryMsg = string.Empty;
            var textMsg = string.Empty;
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
                queryMsg = "Невдача :с";
                textMsg = $"Ви програли!";
                keyboard = InlineKeyboardMarkup.Empty();
            }
            else
            {
                // Creating buttons
                keyboard = keyboardBuilder.BuildDiceSelectionButtons(game.CurrentTurn);

                // Creating bot response
                queryMsg = $"Ви прийняли виклик{player1.FirstName}";
                textMsg = $"@{player2.UserName}(p2) прийняв виклик @{player1.UserName}(p1).\n @{player1.UserName}(p1) ващ хід:";
            }
            try
            {
                await bot.EditMessageText(
                    chatId: player1.ChatId,
                    messageId: query.Message!.Id,
                    text: textMsg,
                    replyMarkup: keyboard);
                await bot.AnswerCallbackQuery(query.Id, queryMsg);
            }
            catch (ApiRequestException ex)
                when (ex.Message.Contains("message is not modified"))
            {

            }
        }
    }
}