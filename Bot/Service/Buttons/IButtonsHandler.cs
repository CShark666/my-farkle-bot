using Telegram.Bot.Types;

namespace Bot
{
    public interface IButtonsHandler
    {
        Task HandleButton(CallbackData callbackData, CallbackQuery query);
    }
}