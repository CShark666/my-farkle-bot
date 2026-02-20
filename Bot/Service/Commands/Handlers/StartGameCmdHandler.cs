using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Service.Commands.Handlers
{
    public class StartGameCmdHandler(
        ITelegramBotClient bot,
        CallbackData callbackData) : ICommandHandler
    {
        private readonly ITelegramBotClient _bot = bot;
        private readonly CallbackData _callbackData = callbackData;
        public async Task HandleCommandAsync(User user)
        {
            var msg = $"@{user.UserName} кинув виклик!";
            var data = _callbackData.Serialize(CallbackActionType.StartGame, user.ChatId, user.UserId);
            var button = InlineKeyboardButton.WithCallbackData(
                text: "Прийняти виклик!",
                callbackData: data);

            await _bot.SendMessage(
                chatId: user.ChatId,
                text: msg,
                replyMarkup: button);
        }
    }
}