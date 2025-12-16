using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class CommandsHandler
    {
        private readonly ILogger _logger;
        private readonly TelegramBotClient _bot;
        private readonly CallbackData _callbackData;
        private readonly Random _random;
        private Dictionary<string, ICommandHandler> _cmdHandler = [];
        public CommandsHandler(ILogger<CommandsHandler> logger, TelegramBotClient bot, CallbackData callbackData, Random random)
        {
            _logger = logger;
            _bot = bot;
            _callbackData = callbackData;
            _random = random;
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
            _cmdHandler["/hello"] = new HelloCommandHandler(_bot,_callbackData);
            _cmdHandler["/play"] = new PlayCmdHandler(_bot, _callbackData, _random);
        }
    }
}