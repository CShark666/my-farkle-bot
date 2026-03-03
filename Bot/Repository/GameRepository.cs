using Microsoft.EntityFrameworkCore;

namespace Bot
{
    public class GameRepository(BotContext db)
    {
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