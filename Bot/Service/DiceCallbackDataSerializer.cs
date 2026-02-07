namespace Bot
{
    public class DiceCallbackDataSerializer
    {
        public string Serialize(
            CallbackActionType actionsType,
            long chatId,
            long userId,
            int buttonId,
            int[] dice,
            List<int> selected)
        {
            return string.Join('|',
                (int)actionsType,
                chatId,
                userId,
                buttonId,
                string.Join(CallbackConfig.ArraySeparator, dice),
                selected is { Count: > 0 } ? string.Join(CallbackConfig.ArraySeparator, selected) : "");
        }
        public void Deserialize(
            string data,
            out CallbackActionType actionsType,
            out long chatId,
            out long userId,
            out int buttonId,
            out int[] diceValues,
            out List<int> selectedDice)
        {
            var p = data.Split('|');

            actionsType = (CallbackActionType)int.Parse(p[0]);
            chatId = long.Parse(p[1]);
            userId = long.Parse(p[2]);
            buttonId = int.Parse(p[3]);
            diceValues = p[4].Split(CallbackConfig.ArraySeparator).Select(int.Parse).ToArray();
            selectedDice = !string.IsNullOrEmpty(p[5])
                ? p[5].Split(CallbackConfig.ArraySeparator).Select(int.Parse).ToList<int>()
                : [];
        }
    }
}