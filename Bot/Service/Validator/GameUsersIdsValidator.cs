using Microsoft.EntityFrameworkCore;

namespace Bot
{
    public class GameUsersIdsValidator(int gameId, long userId, BotContext db, GameResponseFactory responseFactory) : IValidator
    {
        public async Task<ValidationResult> ValidateAsync()
        {
            if (!await IsGameUserValidAsync(gameId, userId))
                return new(false, responseFactory.BuildWrongTurnResponse());
            return new(true);
        }
        private async Task<bool> IsGameUserValidAsync(int gameId, long userId) =>
            await db.Games
                .Where(g => g.Id == gameId)
                .Select(g => g.Player1UserId == userId || g.Player2UserId == userId)
                .FirstOrDefaultAsync();
    }
}