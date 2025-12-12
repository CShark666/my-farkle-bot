using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot
{
    public class ButtonHandler
    {
        private readonly ILogger _logger;
        private readonly TelegramBotClient _bot;
        private Dictionary<InlineBtnsActions, IButtonsHandler> _btnHandler = [];
        public ButtonHandler(ILogger<CommandsHandler> logger, TelegramBotClient bot)
        {
            _logger = logger;
            _bot = bot;
            RegisterButtons();
        }
        public async Task HandleButtonsAsync(CallbackData callbackData, CallbackQuery query)
        {
            var action = callbackData.Action;

            if (_btnHandler.TryGetValue(action, out var handler))
            {
                await handler.HandleButton(callbackData, query);
                _logger.LogInformation("Handled btn action: {btn_action}", handler.GetType().Name);
            }
        }
        private void RegisterButtons()
        {
            _btnHandler[InlineBtnsActions.HelloFirst] = new HelloButtonHandler(_bot);
            _btnHandler[InlineBtnsActions.HelloSecond] = new HelloButtonHandler(_bot);
        }
    }
}