namespace Bot
{
    public interface IDateTimeProvider
    {
        public DateTime Now { get; }
        public DateTime UtcNow { get; }
        public string ToString();
    }

    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;
        public DateTime UtcNow => DateTime.UtcNow;
        public override string ToString() => UtcNow.ToString("dd-MM-yyyy HH:mm");
    }
}