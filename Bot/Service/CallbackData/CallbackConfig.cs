namespace Bot
{
    public static class CallbackConfig
    {
        public const char FieldSeparator = '|';
        public const char ArraySeparator = ',';
        public const int MaxCallbackDataLength = 64;
        public static void ValidateSerializedLength(string data)
        {
            if (data.Length > MaxCallbackDataLength)
                throw new InvalidOperationException(
                $"Serialized callback data ({data.Length} bytes) exceeds " +
                $"Telegram limit of {MaxCallbackDataLength} bytes");
        }
        public static int[] ParseIntArray(string data, char separator = ArraySeparator)
        {
            return string.IsNullOrEmpty(data)
                ? Array.Empty<int>()
                : data.Split(separator).Select(int.Parse).ToArray();
        }

        public static List<int> ParseIntList(string data, char separator = ArraySeparator)
        {
            return string.IsNullOrEmpty(data)
                ? new List<int>()
                : data.Split(separator).Select(int.Parse).ToList();
        }
    }
}