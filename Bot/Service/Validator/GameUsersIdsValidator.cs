using Microsoft.EntityFrameworkCore;

namespace Bot
{
    public class GamePlayersIdsValidator(int gameId, long userId, BotContext db, GameResponseFactory responseFactory) : IValidator
    {
        public async Task<ValidationResult> ValidateAsync()
        {
            if (!await IsGamePlayersIdValidAsync(gameId, userId))
                return new(false, responseFactory.BuildWrongTurnResponse());
            return new(true);
        }
        private async Task<bool> IsGamePlayersIdValidAsync(int gameId, long userId) =>
            await db.Games
                .Where(g => g.Id == gameId)
                .Select(g => g.Player1UserId == userId || g.Player2UserId == userId)
                .FirstOrDefaultAsync();
    }
}