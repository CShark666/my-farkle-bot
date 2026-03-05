namespace Bot
{
    public class Turn
    {
        private const int MaxDice = 6;

        public int Id { get; set; }
        public int GameId { get; set; }
        public long PlayerChatId { get; set; }
        public long PlayerUserId { get; set; }
        public int TurnNumber { get; set; }
        public int TotalScore { get; set; } = 0;
        public int CurrentScore { get; set; } = 0;
        public int[] DiceValue { get; set; } = new int[MaxDice];
        public int[] SelectedDice { get; set; } = [];
        public int RemainingDice { get; private set; } = MaxDice;
        public DateTime CreatedAt { get; set; }
        public Game Game { get; set; }
        public User Player { get; set; }

        public Turn() { }
        public Turn(int gameId, User player)
        {
            GameId = gameId;
            PlayerChatId = player.ChatId;
            PlayerUserId = player.UserId;
            TurnNumber = 1;

            CreatedAt = DateTime.UtcNow;
        }
        public void RollDice()
        {
            for (int i = 0; i < DiceValue.Length; i++)
            {
                DiceValue[i] = Random.Shared.Next(1, 7);
            }
        }
        public void SaveAndRoll()
        {
            SaveCurrentScore();
            ResetDiceState();
            RollDice();
        }
        public void SaveCurrentScore()
        {
            TotalScore += CurrentScore;
            CurrentScore = 0;
        }
        public void ResetDiceState()
        {
            RemainingDice = DiceValue.Length - SelectedDice.Length;

            if (RemainingDice == 0)
                RemainingDice = MaxDice;

            DiceValue = new int[RemainingDice];
            SelectedDice = Array.Empty<int>();
        }
        public void AddOrRemoveDiceSelection(int diceId)
        {
            var tempList = SelectedDice.ToList();

            if (SelectedDice.Contains(diceId))
                tempList.Remove(diceId);
            else
                tempList.Add(diceId);

            SelectedDice = tempList.ToArray();
        }
    }
}