namespace Bot
{
    public class CallbackData
    {
        private const char FieldSeparator = '|';
        private const char ArraySeparator = ',';

        public CallbackActionType Action;
        public long ChatId;
        public long UserId;
        public string Serialize(CallbackActionType actions, long chatId, long userId)
            => $"{actions}{FieldSeparator}{chatId}{FieldSeparator}{userId}";
        public void Deserialize(string encodedString)
        {
            string[] parts = encodedString.Split(FieldSeparator);
            Action = Enum.Parse<CallbackActionType>(parts[0], true);
            ChatId = long.Parse(parts[1]);
            UserId = long.Parse(parts[2]);
        }
        public string DiceSerialize(
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
                string.Join(ArraySeparator, dice),
                selected is { Count: > 0 } ? string.Join(ArraySeparator, selected) : "");
        }
        public void DiceDeserialize(
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
            diceValues = p[4].Split(ArraySeparator).Select(int.Parse).ToArray();
            selectedDice = !string.IsNullOrEmpty(p[5])
                ? p[5].Split(ArraySeparator).Select(int.Parse).ToList<int>()
                : [];
        }
    }
}