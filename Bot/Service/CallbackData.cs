namespace Bot
{
    public class CallbackData
    {
        public InlineBtnsActions Action;
        public long ChatId;
        public long UserId;
        public int[] Dices = new int[6];
        public int ChosenDiceValue;
        private char encodeChar = '*';
        public string EncodeToString(InlineBtnsActions actions, long chatId, long userId)
            => $"{actions}{encodeChar}{chatId}{encodeChar}{userId}";
        public void DecodeFromString(string encodedString)
        {
            string[] parts = encodedString.Split(encodeChar);
            Action = Enum.Parse<InlineBtnsActions>(parts[0], true);
            UserId = long.Parse(parts[1]);
            ChatId = long.Parse(parts[2]);
        }
        public string DiceEncodeToString(InlineBtnsActions actions, long chatId, long userId, int[] dices, int chosenDiceValue)
        {
            var dicesString = string.Join(encodeChar, dices);
            var str = $"{actions}{encodeChar}{chatId}{encodeChar}{userId}{encodeChar}{dicesString}{encodeChar}{chosenDiceValue}";
            return str;
        }
        public void DecodeDiceFromString(string encodedString)
        {
            string[] callbackDataParts = encodedString.Split(encodeChar);
            var partsIndex = 3;

            for (int i = 0; i < 6; i++)
            {
                Dices[i] = int.Parse(callbackDataParts[partsIndex]);
                partsIndex++;
            }

            ChosenDiceValue = int.Parse(callbackDataParts[partsIndex]);
        }
    }
}