using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestApiProject.Models
{
    public class Book
    {

        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Genre { get; set; } = "";
        public int PublishedYear { get; set; }
        public decimal Price { get; set; }

        // Borrowing
        public bool IsBorrowed { get; set; } = false;
        [ForeignKey("BorrowedByUser")]
        public int? BorrowedByUserId { get; set; }   // FK
        public User? BorrowedByUser { get; set; }    // Navigation
        public DateTime? DueDate { get; set; }

        // Navigation
        public List<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();

        // Many-to-Many (Favorites & Wishlist)
        public List<User> FavoritedByUsers { get; set; } = new List<User>();
        public List<User> WishlistedByUsers { get; set; } = new List<User>();

    }
}
