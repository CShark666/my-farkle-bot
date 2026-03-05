namespace Bot
{
    public class GameCleanupService (ILogger<GameCleanupService > logger, BotContext db, GameRepository gameRepository, IDateTimeProvider dateTimeProvider) : BackgroundService
    {
        private const int GameTimeLimit = 15;
        private readonly ILogger<GameCleanupService > _logger = logger;
        private readonly BotContext _db = db;
        private readonly GameRepository _gameRepository = gameRepository;
        private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanupGames();
                await Task.Delay(TimeSpan.FromMinutes(GameTimeLimit), stoppingToken);
            }
        }

        private async Task CleanupGames()
        {
            var time = _dateTimeProvider.UtcNow.AddMinutes(-GameTimeLimit);
            var unfinishedGames = await _gameRepository.GetUnfinishedGamesAsync(time);

            if (unfinishedGames != null)
                foreach (var game in unfinishedGames)
                {
                    game.TechnicalDefeat();
                }
            await _db.SaveChangesAsync();

            _logger.LogInformation("Unfinished games: {unfinishedGames.Count}", unfinishedGames!.Count);
        }
    }
}