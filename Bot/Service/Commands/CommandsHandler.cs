using Telegram.Bot;

namespace Bot
{
    public class CommandsHandler(ILogger<CommandsHandler> logger, TelegramBotClient bot, IEnumerable<ICommandHandler> commandHandlers)
    {
        private readonly ILogger _logger = logger;
        private readonly TelegramBotClient _bot = bot;
        private Dictionary<string, ICommandHandler> _cmdHandler = commandHandlers.ToDictionary(handler => handler.Key);

        public async Task HandleCommandsAsync(string msgText, User user)
        {
            if (_cmdHandler.TryGetValue(msgText, out var handler))
            {
                await handler.HandleCommandAsync(user);
                _logger.LogInformation("Handled command: {command}", handler.GetType().Name);
            }
            else
            {
                _logger.LogError("Unknown command: {command}", msgText);
            }
        }
    }
}