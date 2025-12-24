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
            // InlineKeyboardMarkup inlineKeyboardMarkup = new();
            // var btnsCounter = 0;

            for (int i = 0; i < dices.Length; i++)
            {
                dices[i] = _random.Next(1, 7);
            }

            // for (int i = 0; i < dices.Length; i++)
            // {
            //     var callbackData = _callbackData.DiceEncodeToString(InlineBtnsActions.DicesTesting, user.ChatId, user.UserId, dices, dices[i]);
            //     var btnText = $"{dices[i]} 🔄";

            //     if (btnsCounter % 3 != 0)
            //     {
            //         inlineKeyboardMarkup.AddButton(
            //             InlineKeyboardButton.WithCallbackData(
            //                 btnText,
            //                 callbackData));
            //     }
            //     else
            //     {
            //         inlineKeyboardMarkup.AddNewRow(
            //             InlineKeyboardButton.WithCallbackData(
            //                 btnText,
            //                 callbackData));
            //     }
            //     btnsCounter++;
            // }
            var builder = new BuilderInlineKeyboardMarkups(_callbackData);
            var inlineKeyboardMarkup = builder.BuildDiceKeyboard(dices, user.ChatId, user.UserId);
            await _bot.SendMessage(user.ChatId, msg, replyMarkup: inlineKeyboardMarkup);
        }
    }
}