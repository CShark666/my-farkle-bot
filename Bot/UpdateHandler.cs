using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Bot
{
    public class UpdateHandler(
        ILogger<UpdateHandler> logger,
        CommandsHandler commandsHandler,
        ButtonHandler buttonHandler)
    {
        private readonly ILogger _logger = logger;
        private readonly CommandsHandler _commandsHandler = commandsHandler;
        private readonly ButtonHandler _buttonHandler = buttonHandler;
        public async Task OnUpdateAsync(TelegramBotClient bot, Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    await MessageReceiver(update.Message!);
                    break;
                case UpdateType.CallbackQuery:
                    await CallbackQueryReceiver(update.CallbackQuery!);
                    break;
            }
        }
        private async Task MessageReceiver(Message msg)
        {
            switch (msg.Type)
            {
                case MessageType.Text:
                    await TextMsgReceiver(msg);
                    break;
            }
        }
        private async Task CallbackQueryReceiver(CallbackQuery query)
        {
            var callbackData = new CallbackData();
            callbackData.DecodeFromString(query.Data!);

            await _buttonHandler.HandleButtonsAsync(callbackData, query);
        }
        private async Task TextMsgReceiver(Message msg)
        {
            var msgText = msg.Text!.Split('@')[0].ToLower();
            var user = new User(msg.Chat.Id, msg.From!.Id, msg.From.Username!, msg.From.FirstName);
            
            await _commandsHandler.HandleCommandsAsync(msgText, user);
        }
    }
}