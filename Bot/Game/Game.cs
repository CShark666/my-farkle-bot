namespace Bot
{
    public class Game
    {
        public Guid Id { get; }
        public GameStatus Status { get; private set; }
        public Player Player1 { get; }
        public Player Player2 { get; }
        public long CurrentPlayerId { get; private set; }
        public Turn? CurrentTurn { get; private set; }

        public Game(Player p1, Player p2)
        {
            Id = Guid.NewGuid();
            Player1 = p1;
            Player2 = p2;
            Status = GameStatus.InProgress;
            CurrentPlayerId = p1.UserId;
            CurrentTurn = new Turn();
        }

        public int[] RollDice()
        {
            EnsureGameInProgress();

            CurrentTurn!.Roll();
            return CurrentTurn.DiceValue;
        }

        public void BankScore()
        {
            var player = GetCurrentPlayer();
            player.AddScore(CurrentTurn!.TurnScore);

            EndTurn();
        }

        private void EndTurn()
        {
            CurrentTurn = new Turn();
            CurrentPlayerId = GetOtherPlayer().UserId;
        }

        private Player GetCurrentPlayer() =>
            Player1.UserId == CurrentPlayerId ? Player1 : Player2;

        private Player GetOtherPlayer() =>
            Player1.UserId == CurrentPlayerId ? Player2 : Player1;

        private void EnsureGameInProgress()
        {
            if (Status != GameStatus.InProgress)
                throw new InvalidOperationException("Game finished");
        }
    }
}