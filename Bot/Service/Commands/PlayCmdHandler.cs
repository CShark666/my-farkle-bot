using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class PlayCmdHandler(
        ITelegramBotClient bot,
        CallbackData callbackData,
        DiceService diceService) : ICommandHandler
    {
        private readonly ITelegramBotClient _bot = bot;
        private readonly CallbackData _callbackData = callbackData;
        private readonly DiceService _diceService = diceService;
        public async Task HandleCommandAsync(User user)
        {
            var msg = "Оберіть кубики";
            int[] dices = new int[6];
            dices = _diceService.ThrowDices(dices);
            
            var builder = new BuilderInlineKeyboardMarkups(_callbackData);
            var inlineKeyboardMarkup = builder.BuildDiceKeyboard(dices, user.ChatId, user.UserId);
            
            await _bot.SendMessage(user.ChatId, msg, replyMarkup: inlineKeyboardMarkup);
        }
    }
}