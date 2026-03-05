using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace Bot
{
    public class BotService(
        TelegramBotClient bot,
        ILogger<BotService> logger,
        IServiceProvider serviceProvider,
        IDateTimeProvider dateTimeProvider) : BackgroundService
    {
        private TelegramBotClient _bot = bot;
        private ILogger _logger = logger;
        private IServiceProvider _serviceProvider = serviceProvider;
        private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("It's Alive!!! It's Alive!!! It's Alive!!!"); // Press Ctrl+C to kill it
            _logger.LogInformation("Today is {Date}", _dateTimeProvider);

            _bot.OnError += OnError;
            _bot.OnMessage += OnMessage;
            _bot.OnUpdate += OnUpdate;
         
            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch
            {
                _logger.LogInformation("Bot is stopping...");
            }
        }
        private Task OnError(Exception exception, HandleErrorSource source)
        {
            _logger.LogError(exception, "Bot error from {Source}", source);
            return Task.CompletedTask;
        }
        private async Task OnMessage(Message msg, UpdateType type)
        {
            using var scope = _serviceProvider.CreateScope();
            var cmdHandler = scope.ServiceProvider.GetRequiredService<CommandsHandler>();
            var verifier = scope.ServiceProvider.GetRequiredService<UserRepository>();

            var msgText = msg.Text!.Split('@')[0].ToLower();
            var user = new User(
                chatId: msg.Chat.Id,
                userId: msg.From!.Id,
                userName: msg.From.Username!,
                firstName: msg.From.FirstName);
            
            user = await verifier.GetOrCreateUserAsync(user);

            _logger.LogInformation("Message: from {user}: {text}", user, msgText);

            await cmdHandler.HandleCommandsAsync(msgText, user);
        }
        private async Task OnUpdate(Update update)
        {
            using var scope = _serviceProvider.CreateScope();
            var btnHandler = scope.ServiceProvider.GetRequiredService<ButtonHandler>();
            
            var callbackData = new CallbackData();
            var user = new User(
                chatId: update.CallbackQuery!.Message!.Chat.Id,
                userId: update.CallbackQuery.From!.Id,
                userName: update.CallbackQuery.From.Username!,
                firstName: update.CallbackQuery.From.FirstName);

            callbackData.Deserialize(update.CallbackQuery.Data!);

            _logger.LogInformation("CallbackQuery: from {user}: {text}", user, callbackData.ActionType);

            await btnHandler.HandleButtonsAsync(callbackData, update.CallbackQuery);
        }
    }
}