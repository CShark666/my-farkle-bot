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
            // modelBuilder.Entity<User>()
            //   .HasKey(u => new { u.ChatId, u.UserId });
            modelBuilder.Entity<User>()
                .HasIndex(u => new { u.ChatId, u.UserId })
                .IsUnique();

            // Game
            modelBuilder.Entity<Game>()
            .HasOne(g => g.Player1)
            .WithMany(u => u.GamesAsPlayer1)
            .HasForeignKey(g => g.Player1Id)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.Player2)
                .WithMany(u => u.GamesAsPlayer2)
                .HasForeignKey(g => g.Player2Id)
                .OnDelete(DeleteBehavior.Restrict);

            // Turn -> Game
            modelBuilder.Entity<Turn>()
                .HasOne(t => t.Game)
                .WithMany(g => g.Turns)
                .HasForeignKey(t => t.GameId);

            // Turn -> User (хто ходив)
            modelBuilder.Entity<Turn>()
                .HasOne(t => t.Player)
                .WithMany()
                .HasForeignKey(t => t.PlayerId)
                .OnDelete(DeleteBehavior.Restrict);

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