using Telegram.Bot;

namespace Bot.Service.Commands.Handlers
{
    public class PlayCmdHandler(
        ITelegramBotClient bot,
        CallbackData callbackData,
        DiceService diceService,
        DiceCallbackDataSerializer diceCallbackDataSerializer) : ICommandHandler
    {
        public Task HandleCommandAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}