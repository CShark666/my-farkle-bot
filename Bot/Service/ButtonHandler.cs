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
        private readonly DiceService _diceService;
        private Dictionary<InlineButtonsActions, IButtonsHandler> _btnHandler = [];
        public ButtonHandler(ILogger<CommandsHandler> logger, TelegramBotClient bot, Random random, DiceKeyboardFactory builderInlineKeyboardMarkups, DiceService diceService)
        {
            _logger = logger;
            _bot = bot;
            _random = random;
            _builderInlineKeyboardMarkups = builderInlineKeyboardMarkups;
            _diceService = diceService;
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
            _btnHandler[InlineButtonsActions.HelloFirst] = new HelloButtonHandler(_bot);
            _btnHandler[InlineButtonsActions.HelloSecond] = new HelloButtonHandler(_bot);
            _btnHandler[InlineButtonsActions.Dice] = new PlayBtnHandler(_bot, _builderInlineKeyboardMarkups);
            _btnHandler[InlineButtonsActions.Reroll] = new RerollButtonHandler(_bot, _builderInlineKeyboardMarkups, _diceService);
        }
    }
}