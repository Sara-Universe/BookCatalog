using RestApiProject.Exceptions;
using RestApiProject.Models;
using System.Net;
namespace RestApiProject.Services
{
    public class UserService
    {
        private readonly CsvUserService _csvUserService;
        private readonly CsvBookService _csvBookService;

        private readonly ILogger<UserService> _logger;
        private readonly List<User> _users;
        private readonly List<Book> _books; // Loaded from CsvBookService


        public UserService(CsvUserService csvUserService, CsvBookService csvBookServicem, ILogger<UserService> logger)
        {
            _csvUserService = csvUserService;
            _logger = logger;
            _csvBookService = csvBookServicem;
            // Load users from CSV once
            _users = _csvUserService.LoadUsers();
            _books = _csvBookService.LoadBooks();

        }

        private User GetUserById(int userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                _logger.LogInformation($"User with id: {userId} is not found");

                throw new NotFoundException($"User with id {userId} not found.");
            }
            return user;
        }
        private void ValidateBookExists(int bookId)
        {
            if (!_books.Any(b => b.BookID == bookId))
            {
                _logger.LogInformation($"Book with id: {bookId} is not found");

                throw new ArgumentException($"Book with ID {bookId} does not exist in catalog.");
            }
        }

        // Favorites
        public void AddFavorite(int userId, int bookId)
        {
            ValidateBookExists(bookId); 

            var user = GetUserById(userId);

            if (!user.FavoriteBookIds.Contains(bookId))
            {
                user.FavoriteBookIds.Add(bookId);
                _csvUserService.SaveUsers(_users);
                _logger.LogInformation($"Book {bookId} added to favorites for user {user.Username}");
            }
        }

        public List<int> GetFavorites(int userId)
        {
            var user = GetUserById(userId);
            return user.FavoriteBookIds;
        }

        public void RemoveFavorite(int userId, int bookId)
        {
            ValidateBookExists(bookId); 

            var user = GetUserById(userId);

            if (user.FavoriteBookIds.Remove(bookId))
            {
                _csvUserService.SaveUsers(_users);
                _logger.LogInformation($"Book {bookId} removed from favorites for user {user.Username}");
            }
        }

        // Wishlist
        public void AddToWishlist(int userId, int bookId)
        {
            ValidateBookExists(bookId); 

            var user = GetUserById(userId);

            if (!user.WishlistBookIds.Contains(bookId))
            {
                user.WishlistBookIds.Add(bookId);
                _csvUserService.SaveUsers(_users);
                _logger.LogInformation($"Book {bookId} added to wishlist for user {user.Username}");
            }
        }
        public List<int> GetWishlist(int userId)
        {
            var user = GetUserById(userId);
            return user.WishlistBookIds;
        }

        public void RemoveFromWishlist(int userId, int bookId)
        {
            ValidateBookExists(bookId); 

            var user = GetUserById(userId);

            if (user.WishlistBookIds.Remove(bookId))
            {
                _csvUserService.SaveUsers(_users);
                _logger.LogInformation($"Book {bookId} removed from wishlist for user {user.Username}");
            }
        }

        public void MoveWishlistToFavorites(int userId, int bookId)
        {
            ValidateBookExists(bookId); 

            var user = GetUserById(userId);

            if (user.WishlistBookIds.Remove(bookId))
            {
                if (!user.FavoriteBookIds.Contains(bookId))
                {
                    user.FavoriteBookIds.Add(bookId);
                }
                _csvUserService.SaveUsers(_users);
                _logger.LogInformation($"Book {bookId} moved from wishlist to favorites for user {user.Username}");
            }
        }
    }
 }

