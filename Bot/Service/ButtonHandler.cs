using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot
{
    public class ButtonHandler
    {
        private readonly ILogger _logger;
        private readonly TelegramBotClient _bot;
        private readonly Random _random;
        private readonly DiceKeyboardFactory _builderInlineKeyboardMarkups;
        private Dictionary<InlineBtnsActionsType, IButtonsHandler> _btnHandler = [];
        public ButtonHandler(ILogger<CommandsHandler> logger, TelegramBotClient bot, Random random, DiceKeyboardFactory builderInlineKeyboardMarkups)
        {
            _logger = logger;
            _bot = bot;
            _random = random;
            _builderInlineKeyboardMarkups = builderInlineKeyboardMarkups;
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
            _btnHandler[InlineBtnsActionsType.HelloFirst] = new HelloButtonHandler(_bot);
            _btnHandler[InlineBtnsActionsType.HelloSecond] = new HelloButtonHandler(_bot);
            _btnHandler[InlineBtnsActionsType.DicesTesting] = new PlayBtnHandler(_bot, _builderInlineKeyboardMarkups);
        }
    }
}