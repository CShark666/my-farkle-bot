using Microsoft.EntityFrameworkCore;

namespace Bot
{
    public class UserRepository(ILogger<UserRepository> logger, BotContext db)
    {
        private readonly ILogger<UserRepository> _logger = logger;
        private readonly BotContext _dbContext = db;
        public async Task<User> GetOrCreateUserAsync(User user)
        {
            var existingUser = await _dbContext.Users.FindAsync(user.ChatId, user.UserId);
            if (existingUser == null)
            {
                _logger.LogInformation("Not verified user - {user}", user);

                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();

                return user;
            }
            else
            {
                _logger.LogInformation("Verified user - {userFromDb}", existingUser);
                return existingUser;
            }
        }
    }
}