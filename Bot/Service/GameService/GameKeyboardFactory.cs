using Telegram.Bot.Types.ReplyMarkups;

namespace Bot
{
    public class GameButtonsBuilder(GameCallbackDataSerializer gameCallbackDataSerializer)
    {
        private const int ButtonsPerRow = 3;
        private const string SelectedDiceEmoji = "✅";
        private const string UnselectedDiceEmoji = "🎲";
        private const KeyboardButtonStyle SelectedButton = KeyboardButtonStyle.Success;
        private const KeyboardButtonStyle UnselectedButton = KeyboardButtonStyle.Primary;

        public InlineKeyboardMarkup BuildTurnKeyboard(Turn turn)
        {
            var keyboard = BuildDiceSelectionButtons(turn);
            keyboard.AddNewRow(BuildSaveAndRollButton(turn));
            keyboard.AddNewRow(BuildSaveAndEndButton(turn));

            return keyboard;
        }
        public InlineKeyboardMarkup BuildEndTurnKeyboard(Game game)
        {
            InlineKeyboardMarkup keyboard =
                BuildActionButtonStyle("Почати раунд", CallbackActionType.StartTurn, game.CurrentTurn!, KeyboardButtonStyle.Success);
            keyboard.AddNewRow(
                BuildActionButtonStyle("Здатися.", CallbackActionType.Surrender, game.CurrentTurn!, KeyboardButtonStyle.Danger));

            return keyboard;
        }
        public InlineKeyboardMarkup BuildStartGameKeyboard(string data) =>
            new InlineKeyboardButton("Прийняти виклик!", data) { Style = KeyboardButtonStyle.Success };
        private InlineKeyboardMarkup BuildDiceSelectionButtons(Turn turn)
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
        private InlineKeyboardButton BuildSaveAndRollButton(Turn turn) =>
            BuildActionButton("🔄 Записати очки й продовжити.", CallbackActionType.SaveAndRoll, turn);
        private InlineKeyboardButton BuildSaveAndEndButton(Turn turn) =>
             BuildActionButton("✅ Записати очки й закінчити.", CallbackActionType.SaveAndEnd, turn);
        private InlineKeyboardButton CreateDiceButton(Turn turn, int diceIndex)
        {
            bool isSelected = turn.SelectedDice.Contains(diceIndex);
            var style = isSelected ? SelectedButton : UnselectedButton;

            var text = $"{turn.DiceValue[diceIndex]}";
            var callbackData = gameCallbackDataSerializer.Serialize(
                CallbackActionType.SelectDice,
                turn.PlayerChatId,
                turn.PlayerUserId,
                turn.GameId,
                diceIndex
            );

            return new InlineKeyboardButton(text, callbackData) { Style = style };
        }
        private InlineKeyboardButton BuildActionButton(string text, CallbackActionType actionType, Turn turn)
        {
            var callbackData = gameCallbackDataSerializer.Serialize(
                    actionType, turn.PlayerChatId, turn.PlayerUserId, turn.GameId);
            return InlineKeyboardButton.WithCallbackData(text, callbackData);
        }
        private InlineKeyboardButton BuildActionButtonStyle(string text, CallbackActionType actionType, Turn turn, KeyboardButtonStyle style)
        {
            var callbackData = gameCallbackDataSerializer.Serialize(
                    actionType, turn.PlayerChatId, turn.PlayerUserId, turn.GameId);
            return new InlineKeyboardButton(text, callbackData) { Style = style };
        }

    }
}