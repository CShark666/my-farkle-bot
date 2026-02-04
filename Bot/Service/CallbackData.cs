namespace Bot
{
    public class CallbackData
    {
        public InlineBtnsActionsType Action;
        public long ChatId;
        public long UserId;
        private readonly char encodeChar = '|';
        public string EncodeToString(InlineBtnsActionsType actions, long chatId, long userId)
            => $"{actions}{encodeChar}{chatId}{encodeChar}{userId}";
        public void DecodeFromString(string encodedString)
        {
            string[] parts = encodedString.Split(encodeChar);
            Action = Enum.Parse<InlineBtnsActionsType>(parts[0], true);
            ChatId = long.Parse(parts[1]);
            UserId = long.Parse(parts[2]);
        }
        public string DiceEncodeToString(
            InlineBtnsActionsType actionsType,
            long chatId,
            long userId,
            int buttonId,
            int[] dices,
            List<int> selected)
        {
            return string.Join('|',
                (int)actionsType,
                chatId,
                userId,
                buttonId,
                string.Join(',', dices),
                selected is { Count: > 0 } ? string.Join(',', selected) : "");
        }
        public void Decode(
            string data,
            out InlineBtnsActionsType actionsType,
            out long chatId,
            out long userId,
            out int btnId,
            out int[] dices,
            out List<int> selectedDices)
        {
            var p = data.Split('|');

            actionsType = (InlineBtnsActionsType)int.Parse(p[0]);
            chatId = long.Parse(p[1]);
            userId = long.Parse(p[2]);
            btnId = int.Parse(p[3]);
            dices = p[4].Split(',').Select(int.Parse).ToArray();
            selectedDices = !string.IsNullOrEmpty(p[5])
                ? p[5].Split(',').Select(int.Parse).ToList<int>()
                : [];
        }
    }
}