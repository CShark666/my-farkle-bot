using Telegram.Bot.Types;

namespace Bot
{
    public interface IButtonsHandler
    {
        CallbackActionType Key { get; }
        Task HandleButton(CallbackData callbackData, CallbackQuery query);
    }
}