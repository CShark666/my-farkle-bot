using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot
{
    public class ButtonHandler
    {
        private readonly ILogger _logger;
        private readonly TelegramBotClient _bot;        private readonly DiceKeyboardFactory _builderInlineKeyboardMarkups;
        private readonly DiceService _diceService;
        private readonly DiceCallbackDataSerializer _diceCallbackDataSerializer;
        private Dictionary<CallbackActionType, IButtonsHandler> _btnHandler = [];
        public ButtonHandler(
            ILogger<CommandsHandler> logger,
            TelegramBotClient bot,
            DiceKeyboardFactory builderInlineKeyboardMarkups,
            DiceService diceService,
            DiceCallbackDataSerializer diceCallbackDataSerializer)
        {
            _logger = logger;
            _bot = bot;
            _builderInlineKeyboardMarkups = builderInlineKeyboardMarkups;
            _diceService = diceService;
            _diceCallbackDataSerializer = diceCallbackDataSerializer;
            RegisterButtons();
        }
        public async Task HandleButtonsAsync(CallbackData callbackData, CallbackQuery query)
        {
            if (callbackData.UserId == query.From.Id)
            {
                var action = callbackData.ActionType;
                _btnHandler.TryGetValue(action, out var handler);
                
                await handler!.HandleButton(callbackData, query);
                _logger.LogInformation("Handled btn action: {btn_action}", handler.GetType().Name);

            }
            else
            {
                await _bot.AnswerCallbackQuery(query.Id,
                "❌Це не ваша кнопка.❌");
            }

        }
        private void RegisterButtons()
        {
            _btnHandler[CallbackActionType.HelloFirst] = new HelloButtonHandler(_bot);
            _btnHandler[CallbackActionType.HelloSecond] = new HelloButtonHandler(_bot);
            _btnHandler[CallbackActionType.ThrowDice] = new ThrowDiceBtnHandler(_bot, _builderInlineKeyboardMarkups, _diceCallbackDataSerializer);
            _btnHandler[CallbackActionType.Reroll] = new RerollButtonHandler(_bot, _builderInlineKeyboardMarkups, _diceService);
        }
    }
}