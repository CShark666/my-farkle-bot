namespace Bot
{
    public class User
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public int TotalScore { get; set; } = 0;
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