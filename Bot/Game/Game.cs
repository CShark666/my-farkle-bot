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
    public class Turn
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public long PlayerChatId { get; set; }
        public long PlayerUserId { get; set; }
        public int TurnNumber { get; set; }
        public int DiceId { get; set; } = 0;
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

        public Turn() { }
        public Turn(Game game, User player)
        {
            GameId = game.Id;
            PlayerChatId = player.ChatId;
            PlayerUserId = player.UserId;
            TurnNumber = 1;
        }
        public void RollDice()
        {
            for (int i = 0; i < DiceValue.Length; i++)
            {
                DiceValue[i] = Random.Shared.Next(1, 7);
            }
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

        public void AddSelectedDice(int diceId)
        {
            var tempList = SelectedDice.ToList();
            tempList.Add(diceId);
            SelectedDice = tempList.ToArray();
        }

    }
}
