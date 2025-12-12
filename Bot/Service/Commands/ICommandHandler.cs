namespace Bot
{
    public interface ICommandHandler
    {
        Task HandleCommandAsync(User user);
    }
}