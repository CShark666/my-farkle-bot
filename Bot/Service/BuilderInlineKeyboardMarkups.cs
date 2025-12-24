using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class BuilderInlineKeyboardMarkups(CallbackData callbackData)
    {
        private readonly CallbackData _callbackData = callbackData;
        public InlineKeyboardMarkup BuildDiceKeyboard(int[] dices, long chatId, long userId, int? selectedDice = null)
        {
            var keyboard = new InlineKeyboardMarkup();
            for (int i = 0; i < dices.Length; i++)
            {
                var callbackData = _callbackData.DiceEncodeToString(
                    InlineBtnsActions.DicesTesting,
                    chatId,
                    userId,
                    dices,
                    dices[i]
                );

                var emoji = dices[i] == selectedDice ? "⏹️" : "🔄";
                var button = InlineKeyboardButton.WithCallbackData(
                    $"{dices[i]}{emoji}",
                    callbackData);

                if (i % 3 == 0)
                    keyboard.AddNewRow(button);
                else
                    keyboard.AddButton(button);
            }
            return keyboard;
        }
    }
}