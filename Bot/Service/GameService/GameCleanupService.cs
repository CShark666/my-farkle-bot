namespace Bot
{
    public class GameCleanupService (ILogger<GameCleanupService > logger, IServiceProvider serviceProvider) : BackgroundService
    {
        private const int GameTimeLimit = 15;
        private readonly ILogger<GameCleanupService > _logger = logger;
        private IServiceProvider _serviceProvider = serviceProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanupGames();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task CleanupGames()
        {
            using var scope = _serviceProvider.CreateScope();
            var gameRepository = scope.ServiceProvider.GetRequiredService<GameRepository>();
            var db = scope.ServiceProvider.GetRequiredService<BotContext>();

            var time = DateTime.UtcNow.AddMinutes(-GameTimeLimit);
            var unfinishedGames = await gameRepository.GetUnfinishedGamesAsync(time);

            if (unfinishedGames != null)
                foreach (var game in unfinishedGames)
                {
                    game.TechnicalDefeat();
                }
            await db.SaveChangesAsync();

            _logger.LogInformation("Unfinished games: {unfinishedGames.Count}", unfinishedGames!.Count);
        }
    }
}