using Microsoft.EntityFrameworkCore;
using photo_book_designer_server_main.Data.Models;

namespace photo_book_designer_server_main.Data
{
    public class PhotoBookDBContext : DbContext
    {
        public PhotoBookDBContext(DbContextOptions<PhotoBookDBContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<TgBot> TgBots { get; set; }
        public DbSet<UserRoom> UserRoom { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRoom>()
                .HasKey(ur => new { ur.UserId, ur.RoomId });
        }
    }
}
