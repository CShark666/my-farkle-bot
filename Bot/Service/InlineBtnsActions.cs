namespace Bot
{
    public enum CallbackActionType
    {
        HelloFirst,
        HelloSecond,
        ThrowDice,
        Reroll,
        StartGame,
        SelectDice,
        SaveAndRoll
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