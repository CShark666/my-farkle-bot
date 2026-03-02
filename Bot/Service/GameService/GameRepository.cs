using Microsoft.EntityFrameworkCore;

namespace Bot
{
    public class GameRepository(BotContext db)
    {
        public async Task<bool> IsGameFinishedAsync(int gameId) =>
            await db.Games
                .Where(g => g.Id == gameId)
                .Select(g => g.Status == GameStatus.Finished)
                .FirstOrDefaultAsync();
        
        public async Task<bool> IsGameUserValidAsync(int gameId, long userId) =>
            await db.Games
                .Where(g => g.Id == gameId)
                .Select(g => g.Player1UserId == userId || g.Player2UserId == userId)
                .FirstOrDefaultAsync();
        
        public async Task<Game?> GetGameTurnAsync(int gameId) => 
            await db.Games
                .Include(g => g.CurrentTurn)
                .Include(g => g.CurrentTurn!.Player)
                .FirstOrDefaultAsync(g => g.Id == gameId);

        public async Task<Game?> GetGameAsync(int gameId) =>
            await db.Games
                .Include(g => g.CurrentTurn)
                .Include(g => g.Player1)
                .Include(g => g.Player2)
                .FirstOrDefaultAsync(g => g.Id == gameId);

    }
}