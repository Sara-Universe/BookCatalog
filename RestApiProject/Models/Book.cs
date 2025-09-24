using System.ComponentModel.DataAnnotations;

namespace RestApiProject.Models
{
    public class Book
    {
        
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public int PublishedYear { get; set; }
        public decimal Price { get; set; }


        // Borrowing
        public bool IsBorrowed { get; set; } = false;
        public int? BorrowedByUserId { get; set; }
        public DateTime? DueDate { get; set; }

    }
}
