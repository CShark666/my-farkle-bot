using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class BuilderInlineKeyboardMarkups(CallbackData callbackData)
    {
        private readonly CallbackData _callbackData = callbackData;
        public InlineKeyboardMarkup BuildDiceKeyboard(int[] dices, long chatId, long userId, List<int>? selectedDiceIds = null)
        {
            var keyboard = new InlineKeyboardMarkup();
            for (int i = 0; i < dices.Length; i++)
            {
                List<int> newSelectedDiceIds = [];
                var emoji = "✅";

                if (selectedDiceIds != null)
                {
                    newSelectedDiceIds = new List<int>(selectedDiceIds);
                    if (!selectedDiceIds.Contains(i))
                    {
                        emoji = "🔄";
                        newSelectedDiceIds.Add(i);
                    }
                }
                else
                {
                    emoji = "🔄";
                    newSelectedDiceIds = [i];
                }

                var text = $"{dices[i]} {emoji}";
                var callbackData = _callbackData.DiceEncodeToString(
                    InlineBtnsActionsType.DicesTesting,
                    chatId,
                    userId,
                    i,
                    dices,
                    newSelectedDiceIds
                );

                var button = InlineKeyboardButton.WithCallbackData(text, callbackData);

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