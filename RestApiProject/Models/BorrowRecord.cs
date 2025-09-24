namespace RestApiProject.Models
{
    public class BorrowRecord
    {
        public int Id { get; set; }

        // Foreign Keys
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public string Action { get; set; } = ""; // borrow or return
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }  // only set when borrowing
        public bool IsOverdue { get; set; }     // only for return
    }
}
