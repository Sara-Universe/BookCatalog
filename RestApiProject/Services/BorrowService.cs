using RestApiProject.Exceptions;
using RestApiProject.Models;

namespace RestApiProject.Services
{
    public class BorrowService
    {
        private readonly CsvBookService _csvBookService;
        private readonly CsvUserService _csvUserService;
        private readonly ILogger<BorrowService> _logger;

        private readonly List<Book> _books;
        private readonly List<User> _users;

        private readonly List<BorrowRecord> _history = [];

        public BorrowService(CsvBookService csvBookService, CsvUserService csvUserService, ILogger<BorrowService> logger)
        {
            _csvBookService = csvBookService;
            _csvUserService = csvUserService;
            _logger = logger;

            _books = _csvBookService.LoadBooks();
            _users = _csvUserService.LoadUsers();

        }

        public bool BorrowBook(int userId, int bookId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                _logger.LogInformation($"User with id: {userId} is not found");
                throw new NotFoundException("User not found");
            }

            var book = _books.FirstOrDefault(b => b.BookID == bookId);
            if (book == null)
            {
                _logger.LogInformation($"Book with id: {bookId} is not found");
                throw new NotFoundException("Book not found");
            }

            if (book.IsBorrowed)
            {
                _logger.LogInformation($"Book {book.Title} is already borrowed");
                return false;
            }

            book.IsBorrowed = true;
            book.BorrowedByUserId = userId;
            book.DueDate = DateTime.UtcNow.AddDays(14); 

            _history.Add(new BorrowRecord
            {
                UserId = userId,
                BookId = bookId,
                Action = "Borrow",
                Timestamp = DateTime.UtcNow,
                DueDate = book.DueDate
            });

            _logger.LogInformation($"User {user.Username} borrowed book {book.Title}");

            return true;
        }
        public void ReturnBook(int userId, int bookId)
        {
            var book = _books.FirstOrDefault(b => b.BookID == bookId);
            if (book == null)
            {
                _logger.LogInformation($"Book with id: {bookId} is not found");
                throw new NotFoundException("Book not found");
            }
            if (!_books.Any(b => b.BookID == bookId && b.IsBorrowed && b.BorrowedByUserId == userId))
            {
                if (!book.IsBorrowed)
                {
                    _logger.LogInformation($"Book {book.Title} is not currently borrowed");
                    throw new ValidationException($"Book {book.BookID} is not currently borrowed");
                }

                _logger.LogInformation($"User {userId} cannot return book {book.Title} they did not borrow");
                throw new ValidationException("You cannot return a book that you did not borrow");
            }
            bool isOverdue = book.DueDate.HasValue && DateTime.UtcNow > book.DueDate.Value;

            book.IsBorrowed = false;
            book.BorrowedByUserId = null;
            book.DueDate = null;

            _history.Add(new BorrowRecord
            {
                UserId = userId,
                BookId = bookId,
                Action = "Return",
                Timestamp = DateTime.UtcNow,
                IsOverdue = isOverdue

            });

            _logger.LogInformation($"User {userId} returned book {book.Title}. Overdue: {isOverdue}");
        }

        public List<BorrowRecord> GetBookHistory(int bookId)
        {
            _logger.LogInformation($"Fetching history for book ID: {bookId}");
            return [.. _history.Where(h => h.BookId == bookId)];
        }

        public List<BorrowRecord> GetUserHistory(int userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found when fetching history.");
                throw new NotFoundException("User not found");
            }

            _logger.LogInformation($"Fetching history for user ID: {userId}");
            return _history.Where(h => h.UserId == userId).ToList();
        }

        public List<Book> GetBorrowedBooksByUser(int userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {userId} not found when fetching borrowed books.");
                throw new NotFoundException("User not found");
            }

            _logger.LogInformation($"Fetching borrowed books for user ID: {userId}");
            return _books.Where(b => b.BorrowedByUserId == userId).ToList();
        }

        public List<Book> GetAllOverdueBooks()
        {
            return [.. _books
                .Where(b => b.IsBorrowed
                         && b.DueDate.HasValue
                         && DateTime.UtcNow > b.DueDate.Value)];
        }
    }
}