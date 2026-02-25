namespace Bot
{
    public interface ICommandHandler
    {
        string Key { get; }
        Task HandleCommandAsync(User user);
    }
}