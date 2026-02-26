namespace Bot
{
    public enum CallbackActionType
    {
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
    public enum ButtonType
    {
        DiceKeyboard,
        SaveAndRoll,
        SaveAndEnd
    }
}