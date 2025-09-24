using AutoMapper;
using RestApiProject.DTOs;
using RestApiProject.Exceptions;
using RestApiProject.Models;
namespace RestApiProject.Services
{
    public class UserService
    {
        private readonly CsvUserService _csvUserService;
        private readonly CsvBookService _csvBookService;

        private readonly ILogger<UserService> _logger;
        private readonly List<User> _users;
        private readonly List<Book> _books;
        private readonly IMapper _mapper;


        public UserService(CsvUserService csvUserService, CsvBookService csvBookServicem, ILogger<UserService> logger, IMapper mapper)
        {
            _csvUserService = csvUserService;
            _logger = logger;
            _csvBookService = csvBookServicem;

            _users = _csvUserService.LoadUsers();
            _books = _csvBookService.LoadBooks();
            _mapper = mapper;
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
            if (!_books.Any(b => b.Id == bookId))
            {
                _logger.LogInformation($"Book with id: {bookId} is not found");

                throw new ValidationException($"Book with ID {bookId} does not exist in catalog.");
            }
        }
        // Favorites
        public BookOutputDto? AddFavorite(int userId, int bookId)
        {
            ValidateBookExists(bookId);

            var user = GetUserById(userId);
            var book = _books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                throw new NotFoundException($"Book with id {bookId} not found");
            }

            if (!user.FavoriteBooks.Any(b => b.Id == bookId))

            {

                user.FavoriteBooks.Add(book);
                _csvUserService.SaveUsers(_users);
                _logger.LogInformation($"Book {bookId} added to favorites for user {user.Username}");
                var bookdto = _mapper.Map<BookOutputDto>(book);

                return bookdto;
            }

            return null;
        }

        public List<BookOutputDto> GetFavorites(int userId)
        {
            var user = GetUserById(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            var favoriteBooks = _books
           .Where(b => user.FavoriteBooks.Any(fb => fb.Id == b.Id))
           .ToList();

            return _mapper.Map<List<BookOutputDto>>(favoriteBooks);
        }


        public bool RemoveFavorite(int userId, int bookId)
        {
            ValidateBookExists(bookId);

            var user = GetUserById(userId);
            var book = _books.FirstOrDefault(u => u.Id == bookId);

            if (book != null)
            {


                user.FavoriteBooks.Remove(book);
                _csvUserService.SaveUsers(_users);
                _logger.LogInformation($"Book {bookId} removed from favorites for user {user.Username}");
                return true;
            }
            return false;
        }

        // Wishlist
        public BookOutputDto? AddToWishlist(int userId, int bookId)
        {
            ValidateBookExists(bookId);

            var user = GetUserById(userId);
            var book = _books.FirstOrDefault(b => b.Id == bookId);
            if (book == null)
            {
                throw new NotFoundException($"Book with id {bookId} not found");
            }
            if (!user.FavoriteBooks.Any(b => b.Id == bookId))
            {


                user.WishlistBooks.Add(book);
                _csvUserService.SaveUsers(_users);
                _logger.LogInformation($"Book {bookId} added to wishlist for user {user.Username}");
                var bookdto = _mapper.Map<BookOutputDto>(book);

                return bookdto;
            }

            return null;
        }

        public List<BookOutputDto> GetWishlist(int userId)
        {
            var user = GetUserById(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            var wishlistBooks = _books
            .Where(b => user.WishlistBooks.Any(fb => fb.Id == b.Id))
            .ToList();

            return _mapper.Map<List<BookOutputDto>>(wishlistBooks);
        }


        public bool RemoveFromWishlist(int userId, int bookId)
        {
            ValidateBookExists(bookId);

            var user = GetUserById(userId);

            var book = _books.FirstOrDefault(u => u.Id == bookId);

            if (book != null)
            {


                user.WishlistBooks.Remove(book);
                _csvUserService.SaveUsers(_users);
                _logger.LogInformation($"Book {bookId} removed from favorites for user {user.Username}");
                return true;
            }

            return false;
        }
        public bool MoveWishlistToFavorites(int userId, int bookId)
        {
            ValidateBookExists(bookId);

            var user = GetUserById(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            // Find book in wishlist
            var bookInWishlist = user.WishlistBooks.FirstOrDefault(b => b.Id == bookId);
            if (bookInWishlist == null)
                throw new NotFoundException($"Book with id {bookId} does not exist in the Wishlist");

            // Remove from wishlist
            user.WishlistBooks.Remove(bookInWishlist);

            // Add to favorites if not already there
            if (!user.FavoriteBooks.Any(b => b.Id == bookId))
            {
                user.FavoriteBooks.Add(bookInWishlist);
                _logger.LogInformation($"Book {bookId} moved from wishlist to favorites for user {user.Username}");
                _csvUserService.SaveUsers(_users);  // Save after changes
                return true;
            }
            _csvUserService.SaveUsers(_users);  // Save even if already in favorites
            return false;
        }

    }
}