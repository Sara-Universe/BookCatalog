using Microsoft.EntityFrameworkCore;
using RestApiProject.Models;

namespace RestApiProject.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Favorites
            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteBooks)
                .WithMany(b => b.FavoritedByUsers)
                .UsingEntity(j => j.ToTable("UserFavoriteBooks"));

            // Wishlist
            modelBuilder.Entity<User>()
                .HasMany(u => u.WishlistBooks)
                .WithMany(b => b.WishlistedByUsers)
                .UsingEntity(j => j.ToTable("UserWishlistBooks"));

            // Borrowed books
            modelBuilder.Entity<Book>()
                .HasOne(b => b.BorrowedByUser)
                .WithMany(u => u.BorrowedBooks)
                .HasForeignKey(b => b.BorrowedByUserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Borrow records
            modelBuilder.Entity<BorrowRecord>()
                .HasOne(br => br.Book)
                .WithMany(b => b.BorrowRecords)
                .HasForeignKey(br => br.BookId);

            modelBuilder.Entity<BorrowRecord>()
                .HasOne(br => br.User)
                .WithMany(u => u.BorrowRecords)
                .HasForeignKey(br => br.UserId);
        }

    }
}
