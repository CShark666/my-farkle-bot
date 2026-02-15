using Microsoft.EntityFrameworkCore;

namespace Bot
{
    public class BotContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public string DbPath { get; }
        public BotContext(string dbPath)
        {
            DbPath = dbPath;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // User
            modelBuilder.Entity<User>()
                .HasKey(u => new { u.ChatId, u.UserId });
        }
    }
}