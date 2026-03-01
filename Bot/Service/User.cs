namespace Bot
{
    public class User
    {
        public long ChatId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = "NoName";
        public string FirstName { get; set; } = "NoName";
        public int TotalScore { get; set; } = 0;
        public bool ActiveGames { get; set; } = false;

        public User(long chatId, long userId, string userName, string firstName)
        {
            ChatId = chatId;
            UserId = userId;
            UserName = userName;
            FirstName = firstName;
        }
        public User(long chatId, long userId)
        {
            ChatId = chatId;
            UserId = userId;
        }
        public ICollection<Game>? GamesAsPlayer1 { get; set; }
        public ICollection<Game>? GamesAsPlayer2 { get; set; }

        public override string ToString() => $"[chatId: {ChatId} userId: {UserId}] @{UserName} {FirstName}";
    }
}