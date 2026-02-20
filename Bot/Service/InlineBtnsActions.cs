namespace Bot
{
    public enum CallbackActionType
    {
        HelloFirst,
        HelloSecond,
        ThrowDice,
        Reroll,
        StartGame
    }
    public enum GameStatus
    {
        WaitingOpponent,
        InProgress,
        Finished
    }
    public enum GameCallback
    {
        RollDice,
        SelectDice
    }
}