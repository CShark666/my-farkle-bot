using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class CommandsHandler
    {
        private readonly ILogger _logger;
        private readonly TelegramBotClient _bot;
        private readonly CallbackData _callbackDataCodec;
        private Dictionary<string, ICommandHandler> _cmdHandler = [];
        public CommandsHandler(ILogger<CommandsHandler> logger, TelegramBotClient bot, CallbackData callbackDataCodec)
        {
            _logger = logger;
            _bot = bot;
            _callbackDataCodec = callbackDataCodec;
            RegisterCommands();
        }
        public async Task HandleCommandsAsync(string msgText, User user)
        {
            if(_cmdHandler.TryGetValue(msgText, out var handler))
            {
                await handler.HandleCommandAsync(user);
                _logger.LogInformation("Handled command: {command}", handler.GetType().Name);
            }
            else
            {
                _logger.LogError("Unknown command: {command}", msgText);
            }
        }
        private void RegisterCommands()
        {
            _cmdHandler["/hello"] = new HelloCommandHandler(_bot,_callbackDataCodec);
        }
    }
}