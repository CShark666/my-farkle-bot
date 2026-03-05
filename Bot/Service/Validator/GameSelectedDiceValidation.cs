using Microsoft.EntityFrameworkCore;

namespace Bot
{
    public class GameSelectedDiceValidation(int gameId, BotContext db, GameResponseFactory factory) : IValidator
    {
        public async Task<ValidationResult> ValidateAsync()
        {
            if(await IsSelectedDiceNotNullAsync(gameId))
                return new(false, factory.BuildSelectedDiceIsNull());
            return new(true);
        }

        private async Task<bool> IsSelectedDiceNotNullAsync(int gameId) =>
            await db.Games
                .Where(g => g.Id == gameId)
                .Select(g => g.CurrentTurn!.SelectedDice.Length == 0)
                .FirstOrDefaultAsync();
    }
}