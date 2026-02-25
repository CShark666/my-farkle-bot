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
        SaveAndRoll,
        SaveAndEnd
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