namespace Bot
{
    public class User(long chatId, long userId, string userName, string firstName)
    {
        public int Id { get; set; }
        public long ChatId { get; set; } = chatId;
        public long UserId { get; set; } = userId;
        public string UserName { get; set; } = userName;
        public string FirstName { get; set; } = firstName;
        public int Score { get; set; } = 0;

        public ICollection<Game>? GamesAsPlayer1 { get; set; }
        public ICollection<Game>? GamesAsPlayer2 { get; set; }

        public override string ToString() => $"[chatId: {ChatId} userId: {UserId}] @{UserName} {FirstName}";
    }
}