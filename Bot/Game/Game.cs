using System.ComponentModel.DataAnnotations.Schema;

namespace Bot
{
    public class Game
    {
        public int Id { get; set; }
        public GameStatus Status { get; set; }

        public long Player1ChatId { get; set; }
        public long Player1UserId { get; set; }
        public long Player2ChatId { get; set; }
        public long Player2UserId { get; set; }

        public User Player1 { get; set; }
        public User Player2 { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Turn> Turns { get; set; }
    }

    public class Turn
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public long PlayerChatId { get; set; }
        public long PlayerUserId { get; set; }
        public int TurnNumber { get; set; }
        public int DiceId { get; set; }
        public int[] DiceValue { get; set; } = new int[6];
        public int[] SelectedDice { get; set; } = [];
        [NotMapped]
        public int RemainingDice
        {
            get => SelectedDice.Length > 0
            ? DiceValue.Length - SelectedDice.Length
            : 6;
        }
        public DateTime CreatedAt { get; set; }

        public Game Game { get; set; }
        public User Player { get; set; }
    }
}
