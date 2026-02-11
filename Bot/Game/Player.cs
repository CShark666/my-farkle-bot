namespace Bot
{
    public class Player
    {
        public long UserId { get; private set; }
        public long ChatId { get; private set; }
        public int TotalScore { get; private set; }

        public Player(long id)
        {
            UserId = id;
            TotalScore = 0;
        }

        public void AddScore(int score)
        {
            TotalScore += score;
        }
    }

}