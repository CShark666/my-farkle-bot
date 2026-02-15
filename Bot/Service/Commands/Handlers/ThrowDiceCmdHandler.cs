using Telegram.Bot;

namespace Bot
{
    public class ThrowDiceCmdHandler(
        ITelegramBotClient bot,
        CallbackData callbackData,
        DiceService diceService,
        DiceCallbackDataSerializer diceCallbackDataSerializer) : ICommandHandler
    {
        private readonly ITelegramBotClient _bot = bot;
        private readonly CallbackData _callbackData = callbackData;
        private readonly DiceService _diceService = diceService;
        private readonly DiceCallbackDataSerializer _diceCallbackDataSerializer = diceCallbackDataSerializer;
        public async Task HandleCommandAsync(User user)
        {
            var msg = "Оберіть кубики";
            int[] dice = new int[6];
            dice = _diceService.ThrowDice(dice);
            
            var builder = new DiceKeyboardFactory(_callbackData, _diceCallbackDataSerializer);
            var inlineKeyboardMarkup = builder.BuildDiceSelectionKeyboard(dice, user.ChatId, user.UserId);
            
            await _bot.SendMessage(user.ChatId, msg, replyMarkup: inlineKeyboardMarkup);
        }
    }
}