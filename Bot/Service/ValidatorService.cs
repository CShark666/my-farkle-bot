using Microsoft.EntityFrameworkCore;

namespace Bot
{
    public record ValidationResult(bool IsValid, BotResponse? Response = null);

    public class ValidatorService(BotContext db, GameResponseFactory factory)
    {
        public async Task<ValidationResult> ValidateUserAndGame(long validUserId, long userId, int gameId)
        {
            if (!IsUserIdValid(validUserId, userId)) return new(false, factory.BuildWrongTurnResponse());
            if (await IsGameFinishedAsync(gameId)) return new(false, factory.BuildGameIsFinished());

            return new(true);
        }

        public async Task<ValidationResult> ValidateDiceSelectionAsync(long validUserId, long userId, int gameId)
        {
            if (!IsUserIdValid(validUserId, userId)) return new(false, factory.BuildWrongTurnResponse());
            if (await IsGameFinishedAsync(gameId)) return new(false, factory.BuildGameIsFinished());
            if (await IsSelectedDiceNotNullAsync(gameId)) return new(false, factory.BuildSelectedDiceIsNull());

            return new(true);
        }

        public async Task<ValidationResult> ValidateOpponentsAsync(User player1, User player2)
        {
            if (IsUserIdValid(player1.UserId, player2.UserId)) return new(false, factory.BuildWrongPlayerResponse());
            if (!await IsUsersStatusValid(player1, player2)) return new(false, factory.BuildInvalidUserGamesStatus());
            return new(true);
        }

        public async Task<ValidationResult> ValidateGameStatus(int gameId)
        {
            if (await IsGameFinishedAsync(gameId)) return new(false, factory.BuildGameIsFinished());
            return new(true);
        }

        public async Task<ValidationResult> ValidateUsersAndGameAsync(int gameId, long userId)
        {
            if (!await IsGamePlayersIdValidAsync(gameId, userId)) return new(false, factory.BuildGameIsFinished());
            if (await IsGameFinishedAsync(gameId)) return new(false, factory.BuildGameIsFinished());
            return new(true);
        }
        private bool IsUserIdValid(long validUserId, long userId) =>
            validUserId == userId;
        private async Task<bool> IsGameFinishedAsync(int gameId) =>
            await db.Games
                .Where(g => g.Id == gameId)
                .Select(g => g.Status == GameStatus.Finished)
                .FirstOrDefaultAsync();
        private async Task<bool> IsGamePlayersIdValidAsync(int gameId, long userId) =>
            await db.Games
                .Where(g => g.Id == gameId)
                .Select(g => g.Player1UserId == userId || g.Player2UserId == userId)
                .FirstOrDefaultAsync();
        private async Task<bool> IsSelectedDiceNotNullAsync(int gameId) =>
            await db.Games
                .Where(g => g.Id == gameId)
                .Select(g => g.CurrentTurn!.SelectedDice.Length == 0)
                .FirstOrDefaultAsync();
        private async Task<bool> IsUsersStatusValid(User player1, User player2)
        {
            return await db.Users
                .Where(u =>
                    (u.ChatId == player1.ChatId && u.UserId == player1.UserId) ||
                    (u.ChatId == player2.ChatId && u.UserId == player2.UserId))
                .CountAsync(u => u.ActiveGames == false) == 2;
        }
    }
}