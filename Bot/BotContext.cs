using Microsoft.EntityFrameworkCore;

namespace Bot
{
    public class BotContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Turn> Turns { get; set; }

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
            // Game
            modelBuilder.Entity<Game>()
                .HasOne(g => g.Player1)
                .WithMany(u => u.GamesAsPlayer1)
                .HasForeignKey(g => new { g.Player1ChatId, g.Player1UserId })
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Game>()
                .HasOne(g => g.Player2)
                .WithMany(u => u.GamesAsPlayer2)
                .HasForeignKey(g => new { g.Player2ChatId, g.Player2UserId })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.CurrentTurn)
                .WithOne()
                .HasForeignKey<Game>(g => g.CurrentTurnId)
                .OnDelete(DeleteBehavior.Restrict);
            // Turn -> Game
            modelBuilder.Entity<Turn>()
                .HasOne(t => t.Game)
                .WithMany(g => g.Turns)
                .HasForeignKey(t => t.GameId);
            // Turn -> User
            modelBuilder.Entity<Turn>()
                .HasOne(t => t.Player)
                .WithMany()
                .HasForeignKey(t => new { t.PlayerChatId, t.PlayerUserId })
                .OnDelete(DeleteBehavior.Restrict);

            // Turn - Dice
            modelBuilder.Entity<Turn>()
                .Property(t => t.DiceValue)
                    .HasConversion(
                        dv => string.Join(',', dv),
                        dv => dv.Split(',').Select(int.Parse).ToArray()
                    );
            modelBuilder.Entity<Turn>()
                .Property(t => t.SelectedDice)
                    .HasConversion(
                        sd => string.Join(',', sd),
                        sd => sd == "" ? new int[0] : sd.Split(',').Select(int.Parse).ToArray()
                    );
        }
    }
}