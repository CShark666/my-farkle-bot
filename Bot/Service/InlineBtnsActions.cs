namespace Bot
{
    public enum CallbackActionType
    {
        HelloFirst,
        HelloSecond,
        ThrowDice,
        Reroll,
        StartGame,
        SelectDice
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