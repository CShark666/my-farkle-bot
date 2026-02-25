using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.Service.Commands.Handlers
{
    public class StartGameCmdHandler(
        TelegramBotClient bot,
        CallbackData callbackData) : ICommandHandler
    {
        public string Key => "/startgame";

        private readonly TelegramBotClient _bot = bot;
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