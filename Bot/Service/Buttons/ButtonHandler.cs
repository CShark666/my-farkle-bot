using Telegram.Bot;
using Telegram.Bot.Types;

namespace Bot
{
    public class ButtonHandler(ILogger<CommandsHandler> logger, TelegramBotClient bot, IEnumerable<IButtonsHandler> buttonsHandlers)
    {
        private readonly ILogger _logger = logger;
        private readonly TelegramBotClient _bot = bot;
        private Dictionary<CallbackActionType, IButtonsHandler> _btnHandler = buttonsHandlers.ToDictionary(handler => handler.Key);

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
    }
}