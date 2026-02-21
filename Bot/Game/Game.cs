using System.ComponentModel.DataAnnotations.Schema;

namespace Bot
{
    public class Game
    {
        public int Id { get; set; }
        public GameStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
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
        public ICollection<Turn> Turns { get; set; }
        public Game() { }
        public Game(User p1, User p2, DateTime createdDateTime)
        {
            Status = GameStatus.InProgress;
            Player1 = p1;
            Player2 = p2;
            CreatedAt = createdDateTime;
        }
    }
}
