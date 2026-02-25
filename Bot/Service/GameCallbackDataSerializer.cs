namespace Bot
{
    public class GameCallbackDataSerializer
    {
        public string Serialize(
            CallbackActionType actionType,
            long playerChatId,
            long playerUserId,
            int gameId,
            int diceIndex
        )
        {
            return string.Join('|',
                    (int)actionType,
                    playerChatId,
                    playerUserId,
                    gameId,
                    diceIndex);
        }
        public void Deserialize(string data, out int gameId, out int diceIndex)
        {
            var p = data.Split('|');
            gameId = int.Parse(p[3]);
            diceIndex = int.Parse(p[4]);
        }
        // NO DICE ID
        public string Serialize(
            CallbackActionType actionType,
            long playerChatId,
            long playerUserId,
            int gameId
        )
        {
            return string.Join('|',
                    (int)actionType,
                    playerChatId,
                    playerUserId,
                    gameId);
        }
        public void Deserialize(string data, out int gameId)
        {
            var p = data.Split('|');
            gameId = int.Parse(p[3]);
        }
        public void Deserialize(string data,
            out CallbackActionType actionType,
            out long playerChatId,
            out long playerUserId,
            out int gameId,
            out int diceIndex)
        {
            var p = data.Split('|');
            actionType = (CallbackActionType)int.Parse(p[0]);
            playerChatId = long.Parse(p[1]);
            playerUserId = long.Parse(p[2]);
            gameId = int.Parse(p[3]);
            diceIndex = int.Parse(p[4]);
        }
    }
}