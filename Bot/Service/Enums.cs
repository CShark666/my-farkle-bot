namespace Bot
{
    public enum CallbackActionType
    {
        StartGame,
        SelectDice,
        SaveAndRoll,
        SaveAndEnd,
        StartTurn,
        Surrender
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