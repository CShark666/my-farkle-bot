using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class BuilderInlineKeyboardMarkups(CallbackData callbackData)
    {
        private readonly CallbackData _callbackData = callbackData;
        public InlineKeyboardMarkup BuildDiceKeyboard(int[] dices, long chatId, long userId)
        {
            var keyboard = new InlineKeyboardMarkup();
            for (int i = 0; i < dices.Length; i++)
            {
                List<int> selectedDice = [i];

                var callbackData = _callbackData.DiceEncodeToString(
                    InlineBtnsActionsType.DicesTesting,
                    chatId,
                    userId,
                    i,
                    dices,
                    selectedDice!
                );

                var emoji = "🔄";
                var button = InlineKeyboardButton.WithCallbackData(
                    $"{dices[i]}{emoji}",
                    callbackData);

                if (i % 3 == 0)
                    keyboard.AddNewRow(button);
                else
                    keyboard.AddButton(button);
            }
            keyboard.AddNewRow().AddButton(InlineKeyboardButton.WithCallbackData("Перекинути", "null"));
            return keyboard;
        }
    }
}