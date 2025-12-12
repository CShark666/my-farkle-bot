namespace Bot
{
    public class User
    {
        public long ChatId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; } = "NO_USER_NAME";
        public string FirstName { get; set; } = "NO_FIRST_NAME";
        public int Score { get; set; } = 0;

        public User(long chatId, long userId, string userName, string firstName)
        {
            ChatId = chatId;
            UserId = userId;
            UserName = userName;
            FirstName = firstName;
        }
    }
}