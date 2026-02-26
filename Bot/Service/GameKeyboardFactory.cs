using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class GameButtonsBuilder(GameCallbackDataSerializer gameCallbackDataSerializer)
    {
        private const int ButtonsPerRow = 3;
        private const string SelectedDiceEmoji = "✅";
        private const string UnselectedDiceEmoji = "🎲";

        public InlineKeyboardMarkup BuildDiceSelectionButtons(Turn turn)
        {
            var keyboard = new InlineKeyboardMarkup();

            for (int i = 0; i < turn.DiceValue.Length; i++)
            {
                var button = CreateDiceButton(turn, i);

                if (i % ButtonsPerRow == 0)
                    keyboard.AddNewRow(button);
                else
                    keyboard.AddButton(button);
            }

            return keyboard;
        }
        public InlineKeyboardButton BuildSaveAndRollButton(Turn turn) =>
            BuildActionButton("🔄 Записати очки й продовжити.", CallbackActionType.SaveAndRoll, turn);
        public InlineKeyboardButton BuildSaveAndEndButton(Turn turn) =>
             BuildActionButton("✅ Записати очки й закінчити.", CallbackActionType.SaveAndEnd, turn);
        private InlineKeyboardButton CreateDiceButton(Turn turn, int diceIndex)
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
        private InlineKeyboardButton BuildActionButton(string text, CallbackActionType actionType, Turn turn)
        {
            var callbackData = gameCallbackDataSerializer.Serialize(
                    actionType, turn.PlayerChatId, turn.PlayerUserId, turn.GameId);
            return InlineKeyboardButton.WithCallbackData(text, callbackData);
        }
    }
}