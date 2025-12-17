using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class PlayCmdHandler(ITelegramBotClient bot, CallbackData callbackData, Random random) : ICommandHandler
    {
        private readonly ITelegramBotClient _bot = bot;
        private readonly CallbackData _callbackData = callbackData;
        private readonly Random _random = random;
        public async Task HandleCommandAsync(User user)
        {
            var msg = "Оберіть кубики";
            int[] dices = new int[6];
            InlineKeyboardMarkup inlineKeyboardMarkup = new();

            for(int i = 0; i < dices.Length; i++)
            {
                dices[i] = _random.Next(1, 7);
            }

            for (int i = 0; i < dices.Length; i++)
            {
                var callbackData = _callbackData.DiceEncodeToString(InlineBtnsActions.DicesTesting, user.ChatId, user.UserId, dices);
                inlineKeyboardMarkup.AddButton(
                    InlineKeyboardButton.WithCallbackData(
                        $"{dices[i]} ⏹️",
                        callbackData));
            }

            await _bot.SendMessage(user.ChatId, msg, replyMarkup: inlineKeyboardMarkup);
        }
    }
}