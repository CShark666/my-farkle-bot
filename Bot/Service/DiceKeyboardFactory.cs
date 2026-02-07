using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class DiceKeyboardFactory(CallbackData callbackData, DiceCallbackDataSerializer diceCallbackDataSerializer)
    {
        private readonly CallbackData _callbackData = callbackData;
        private const int ButtonsPerRow = 3;
        private const string SelectedDiceEmoji = "✅";
        private const string UnselectedDiceEmoji = "🔄";

        public InlineKeyboardMarkup BuildDiceSelectionKeyboard(
            int[] diceValues,
            long chatId,
            long userId,
            List<int>? selectedDiceIndices = null)
        {
            if (diceValues == null || diceValues.Length == 0)
                throw new ArgumentException("Dice array cannot be null or empty", nameof(diceValues));

            selectedDiceIndices ??= new List<int>();
            var keyboard = new InlineKeyboardMarkup();

            for (int i = 0; i < diceValues.Length; i++)
            {
                var button = CreateDiceToggleButton(chatId, userId, i, diceValues, selectedDiceIndices);

                if (i % ButtonsPerRow == 0)
                    keyboard.AddNewRow(button);
                else
                    keyboard.AddButton(button);
            }
            keyboard.AddNewRow(CreateRerollDiceButton(chatId, userId));
            return keyboard;
        }

        private InlineKeyboardButton CreateRerollDiceButton(long chatId, long userId)
        {
            var callbackData = _callbackData.Serialize(CallbackActionType.Reroll, chatId, userId);
            var text = "Перекинути кубики";
            return InlineKeyboardButton.WithCallbackData(text, callbackData);
        }
        private InlineKeyboardButton CreateDiceToggleButton(
            long chatId,
            long userId,
            int diceIndex,
            int[] allDiceValues,
            List<int> currentlySelectedIndices)
        {
            bool isSelected = currentlySelectedIndices.Contains(diceIndex);
            var emoji = isSelected ? SelectedDiceEmoji : UnselectedDiceEmoji;
            var newSelectedDiceIds = ToggleDiceSelection(diceIndex, currentlySelectedIndices);
            var text = $"{allDiceValues[diceIndex]} {emoji}";
            var callbackData = diceCallbackDataSerializer.Serialize(
                CallbackActionType.Dice,
                chatId,
                userId,
                diceIndex,
                allDiceValues,
                newSelectedDiceIds
            );

            return InlineKeyboardButton.WithCallbackData(text, callbackData);
        }
        private List<int> ToggleDiceSelection(int diceIndex, List<int> currentSelected)
        {
            var updatedSelection = new List<int>(currentSelected);

            if (currentSelected.Contains(diceIndex))
                updatedSelection.Remove(diceIndex);
            else
                updatedSelection.Add(diceIndex);

            return updatedSelection;
        }
    }
}