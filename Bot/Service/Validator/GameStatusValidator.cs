using Microsoft.EntityFrameworkCore;

namespace Bot
{
    public class GameStatusValidator(int gameId, BotContext db, GameResponseFactory factory) : IValidator
    {
        public async Task<ValidationResult> ValidateAsync()
        {
            if (await IsGameFinishedAsync(gameId))
                return new(false, factory.BuildGameIsFinished());
            return new(true);
        }
        private async Task<bool> IsGameFinishedAsync(int gameId) =>
            await db.Games
                .Where(g => g.Id == gameId)
                .Select(g => g.Status == GameStatus.Finished)
                .FirstOrDefaultAsync();
    }
}