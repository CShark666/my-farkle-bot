using System.ComponentModel.DataAnnotations.Schema;

namespace Bot
{
    public class Game
    {
        public int Id { get; set; }
        public GameStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public User? Winner { get; set; }
        public long Player1ChatId { get; set; }
        public long Player1UserId { get; set; }
        public long Player2ChatId { get; set; }
        public long Player2UserId { get; set; }
        public User Player1 { get; set; }
        public User Player2 { get; set; }
        public int? CurrentTurnId { get; set; }
        [NotMapped]
        public long? CurrentPlayerId { get => CurrentTurn!.PlayerUserId; }
        public Turn? CurrentTurn { get; set; }
        [NotMapped]
        public int GameGoalScore = 10000;
        public ICollection<Turn> Turns { get; set; } = new List<Turn>();
        public Game() { }
        public Game(User p1, User p2, DateTime createdDateTime)
        {
            Status = GameStatus.InProgress;
            Player1 = p1;
            Player2 = p2;
            
            Player1.TotalScore = 0;
            Player1.ActiveGames = true;

            Player2.TotalScore = 0;
            Player2.ActiveGames = true;

            CreatedAt = createdDateTime;
        }

        public User GetOpponent()
        {
            return CurrentPlayerId == Player1.UserId ? Player2 : Player1;
        }

        public void StartTurn(User player)
        {
            var turn = new Turn(Id, player);
            Turns.Add(turn);
            CurrentTurn = turn;
        }
        public User? GetOpponentWithId(long userId) =>
            Player1.UserId == userId
            ? Player2
            : Player1;

        public void FinishTurn()
        {
            CurrentTurn!.SaveCurrentScore();
            CurrentTurn.Player.TotalScore += CurrentTurn.TotalScore;
        }
        public void FinishGame()
        {
            Status = GameStatus.Finished;
            Winner = CurrentTurn!.Player;
            
            Player1.ActiveGames = false;
            Player2.ActiveGames = false;
        }
        public void TechnicalDefeat()
        {
            Status = GameStatus.Finished;
            Player1.ActiveGames = false;
            Player2.ActiveGames = false;
        }
        public bool IsPlayerWins() =>
            CurrentTurn!.Player.TotalScore >= GameGoalScore;
    }
}
