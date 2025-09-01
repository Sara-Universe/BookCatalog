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
            if (!_books.Any(b => b.BookID == bookId))
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

            if (!user.FavoriteBookIds.Contains(bookId))
            {
                user.FavoriteBookIds.Add(bookId);
                _csvUserService.SaveUsers(_users);
                _logger.LogInformation($"Book {bookId} added to favorites for user {user.Username}");

                var book = _books.FirstOrDefault(b => b.BookID == bookId);
                return _mapper.Map<BookOutputDto>(book);
            }

            return null;
        }

        public List<BookOutputDto> GetFavorites(int userId)
        {
            var user = GetUserById(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            var favoriteBooks = _books
                .Where(b => user.FavoriteBookIds.Contains(b.BookID))
                .ToList();

            return _mapper.Map<List<BookOutputDto>>(favoriteBooks);
        }


        public bool RemoveFavorite(int userId, int bookId)
        {
            ValidateBookExists(bookId); 

            var user = GetUserById(userId);

            if (user.FavoriteBookIds.Remove(bookId))
            {
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

            if (!user.WishlistBookIds.Contains(bookId))
            {
                user.WishlistBookIds.Add(bookId);
                _csvUserService.SaveUsers(_users);
                _logger.LogInformation($"Book {bookId} added to wishlist for user {user.Username}");

                var book = _books.FirstOrDefault(b => b.BookID == bookId);
                return _mapper.Map<BookOutputDto>(book);
            }

            return null;
        }

        public List<BookOutputDto> GetWishlist(int userId)
        {
            var user = GetUserById(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            var wishlistBooks = _books
                .Where(b => user.WishlistBookIds.Contains(b.BookID))
                .ToList();

            return _mapper.Map<List<BookOutputDto>>(wishlistBooks);
        }


        public bool RemoveFromWishlist(int userId, int bookId)
        {
            ValidateBookExists(bookId); 

            var user = GetUserById(userId);

            if (user.WishlistBookIds.Remove(bookId))
            {
                _csvUserService.SaveUsers(_users);
                _logger.LogInformation($"Book {bookId} removed from wishlist for user {user.Username}");
                return true;
            }
            return false;
        }

        //move book from wishlist to favorites
        public bool MoveWishlistToFavorites(int userId, int bookId)
        {
            ValidateBookExists(bookId); 

            var user = GetUserById(userId);
            bool flag = user.WishlistBookIds.Remove(bookId);
            if (!flag)
                throw new NotFoundException($"Book with id {bookId} does not exist in the Whishlist");

                if (!user.FavoriteBookIds.Contains(bookId))
                {
                    user.FavoriteBookIds.Add(bookId);
                    _logger.LogInformation($"Book {bookId} moved from wishlist to favorites for user {user.Username}");
                     return true;

                 }
           _csvUserService.SaveUsers(_users);
            
            return false;
        }
    }
 }