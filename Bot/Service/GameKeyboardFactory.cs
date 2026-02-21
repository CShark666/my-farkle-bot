using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class GameKeyboardFactory(GameCallbackDataSerializer gameCallbackDataSerializer)
    {
        private const int ButtonsPerRow = 3;
        private const string SelectedDiceEmoji = "✅";
        private const string UnselectedDiceEmoji = "🔄";

        public InlineKeyboardMarkup BuildDiceSelectionKeyboard(Turn turn)
        {
            var keyboard = new InlineKeyboardMarkup();

            for (int i = 0; i < turn.DiceValue.Length; i++)
            {
                var button = CreateDiceToggleButton(turn, i);

                if (i % ButtonsPerRow == 0)
                    keyboard.AddNewRow(button);
                else
                    keyboard.AddButton(button);
            }

            return keyboard;
        }

        private InlineKeyboardButton CreateDiceToggleButton(Turn turn, int diceIndex)
        {
            bool isSelected = turn.SelectedDice.Contains(diceIndex);
            var emoji = isSelected ? SelectedDiceEmoji : UnselectedDiceEmoji;

            var text = $"{turn.DiceValue[diceIndex]} {emoji}";
            var callbackData = gameCallbackDataSerializer.Serialize(
                CallbackActionType.SelectDice,
                turn.PlayerChatId,
                turn.PlayerUserId,
                turn.GameId,
                diceIndex
            );

            return InlineKeyboardButton.WithCallbackData(text, callbackData);
        }
    }
}