namespace Bot
{
    public class CallbackData
    {
        public long ChatId;
        public long UserId;
        public InlineBtnsActions Action;
        private char encodeChar = '*';
        public string EncodeToString(InlineBtnsActions actions,long chatId, long userId)
            => $"{actions}{encodeChar}{chatId}{encodeChar}{userId}";
        public void DecodeFromString(string encodedString)
        {
            string[] parts = encodedString.Split(encodeChar);
            Action = Enum.Parse<InlineBtnsActions>(parts[0], true);
            UserId = long.Parse(parts[1]);
            ChatId = long.Parse(parts[2]);
        }
    }
}