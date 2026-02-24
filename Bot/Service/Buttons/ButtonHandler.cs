using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot
{
    public class ButtonHandler
    {
        private readonly ILogger _logger;
        private readonly TelegramBotClient _bot;
        private readonly DiceKeyboardFactory _builderInlineKeyboardMarkups;
        private readonly DiceService _diceService;
        private readonly DiceCallbackDataSerializer _diceCallbackDataSerializer;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly BotContext _botContext;
        private readonly UserRepository _userRepository;
        private readonly GameKeyboardFactory _gameKeyboardFactory;
        private readonly GameCallbackDataSerializer _gameCallbackDataSerializer;
        private readonly ScoreCalculator _scoreCalculator;
        private Dictionary<CallbackActionType, IButtonsHandler> _btnHandler = [];
        public ButtonHandler(
            ILogger<CommandsHandler> logger,
            TelegramBotClient bot,
            DiceKeyboardFactory builderInlineKeyboardMarkups,
            DiceService diceService,
            DiceCallbackDataSerializer diceCallbackDataSerializer,
            IDateTimeProvider dateTimeProvider,
            BotContext botContext,
            UserRepository userRepository,
            GameKeyboardFactory gameKeyboardFactory,
            GameCallbackDataSerializer gameCallbackDataSerializer,
            ScoreCalculator scoreCalculator)
        {
            _logger = logger;
            _bot = bot;
            _builderInlineKeyboardMarkups = builderInlineKeyboardMarkups;
            _diceService = diceService;
            _diceCallbackDataSerializer = diceCallbackDataSerializer;
            _dateTimeProvider = dateTimeProvider;
            _botContext = botContext;
            _userRepository = userRepository;
            _gameKeyboardFactory = gameKeyboardFactory;
            _gameCallbackDataSerializer = gameCallbackDataSerializer;
            _scoreCalculator = scoreCalculator;
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
            _btnHandler[CallbackActionType.StartGame] = new StartGameBtnHandler(_bot, _dateTimeProvider, _botContext, _userRepository, _gameKeyboardFactory);
            _btnHandler[CallbackActionType.SelectDice] = new SelectDiceBtnHandler(_bot, _botContext, _gameKeyboardFactory, _gameCallbackDataSerializer, _scoreCalculator);
            _btnHandler[CallbackActionType.SaveAndRoll] = new SaveAndRollBtnHandler(_bot, _botContext, _gameKeyboardFactory, _gameCallbackDataSerializer, _scoreCalculator);
            _btnHandler[CallbackActionType.SaveAndEnd] = new SaveAndEndBtnHandler(_bot, _botContext, _gameKeyboardFactory, _gameCallbackDataSerializer, _scoreCalculator);
        }
    }
}