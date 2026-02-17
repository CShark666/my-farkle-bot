using Bot.Service.Commands.Handlers;
using Telegram.Bot;

namespace Bot
{
    public class CommandsHandler
    {
        private readonly ILogger _logger;
        private readonly TelegramBotClient _bot;
        private readonly CallbackData _callbackData;
        private readonly DiceService _diceService;
        private readonly DiceCallbackDataSerializer _diceCallbackDataSerializer;
        private Dictionary<string, ICommandHandler> _cmdHandler = [];
        public CommandsHandler(
            ILogger<CommandsHandler> logger,
            TelegramBotClient bot,
            CallbackData callbackData,
            DiceService diceService,
            DiceCallbackDataSerializer diceCallbackDataSerializer)
        {
            _logger = logger;
            _bot = bot;
            _callbackData = callbackData;
            _diceService = diceService;
            _diceCallbackDataSerializer = diceCallbackDataSerializer;
            
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
            _cmdHandler["/throwdice"] = new ThrowDiceCmdHandler(_bot, _callbackData, _diceService, _diceCallbackDataSerializer);
            _cmdHandler["/play"] = new PlayCmdHandler(_bot, _callbackData, _diceService, _diceCallbackDataSerializer);
        }
    }
}