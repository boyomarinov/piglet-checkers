namespace PigletCheckers.Data
{
    using System.Data.Entity;
    using PigletCheckers.Models;

    public class GameDbContext : DbContext
    {
        public GameDbContext()
            : base("GameDb")
        {
        }

        public DbSet<Game> Games { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Piece> Pieces { get; set; }

        public DbSet<UserMessage> UserMessages { get; set; }
    }
}
