using System.ComponentModel.DataAnnotations.Schema;

namespace Bot
{
    public class Game
    {
        public int Id { get; set; }
        public GameStatus Status { get; set; }

        public int Player1Id { get; set; }
        public int Player2Id { get; set; }

        public User Player1 { get; set; }
        public User Player2 { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Turn> Turns { get; set; }
    }

    public class Turn
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int PlayerId { get; set; }
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
