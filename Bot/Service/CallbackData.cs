namespace Bot
{
    public class CallbackData
    {
        public CallbackActionType ActionType { get; set; }
        public long ChatId { get; set; }
        public long UserId { get; set; }
        public string Serialize(CallbackActionType actions, long chatId, long userId)
        {
            var result = string.Join(CallbackConfig.FieldSeparator, (int)actions, chatId, userId);
            CallbackConfig.ValidateSerializedLength(result);
            return result;
        }
        public void Deserialize(string data)
        {
            var fields = data.Split(CallbackConfig.FieldSeparator);

            ActionType = (CallbackActionType)int.Parse(fields[0]);
            ChatId = long.Parse(fields[1]);
            UserId = long.Parse(fields[2]);
        }
    }
}