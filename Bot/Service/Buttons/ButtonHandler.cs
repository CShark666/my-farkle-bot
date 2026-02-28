using Telegram.Bot.Types;

namespace Bot
{
    public class ButtonHandler(ILogger<CommandsHandler> logger, IEnumerable<IButtonsHandler> buttonsHandlers)
    {
        private Dictionary<CallbackActionType, IButtonsHandler> _btnHandler = buttonsHandlers.ToDictionary(handler => handler.Key);

        public async Task HandleButtonsAsync(CallbackData callbackData, CallbackQuery query)
        {
            var action = callbackData.ActionType;
            _btnHandler.TryGetValue(action, out var handler);

            await handler!.HandleButton(callbackData, query);
            logger.LogInformation("Handled btn action: {btn_action}", handler.GetType().Name);
        }
    }
}