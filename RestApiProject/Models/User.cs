using RestApiProject.DTOs;
namespace RestApiProject.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "";

        // Borrowed books (One-to-Many)
        public List<Book> BorrowedBooks { get; set; } = new List<Book>();

        // Borrow history
        public List<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();

        // Many-to-Many (Favorites & Wishlist)
        public List<Book> FavoriteBooks { get; set; } = new List<Book>();
        public List<Book> WishlistBooks { get; set; } = new List<Book>();
    }
}
