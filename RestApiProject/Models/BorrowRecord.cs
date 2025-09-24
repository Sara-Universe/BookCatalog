namespace RestApiProject.Models
{
    public class BorrowRecord
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
        public string Action { get; set; } // borrow or return
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }  // only set when borrowing
        public bool IsOverdue { get; set; }     // only for return
    }
}
