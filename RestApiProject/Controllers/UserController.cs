using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestApiProject.Services;
using System.Security.Claims;

namespace RestApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(UserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User ID claim missing in token.");

            return int.Parse(userIdClaim);
        }

        private void EnsureCurrentUser(int userId)
        {
            var currentUserId = GetCurrentUserId();
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            // Admin can access anyone, normal users only themselves
            if (role == "User" && currentUserId != userId)
            {
                throw new UnauthorizedAccessException("You cannot access another user's data.");
            }
        }

        // Favorites
        [HttpPost("{userId}/favorites/{bookId}")]
        public IActionResult AddFavorite(int userId, int bookId)
        {
            EnsureCurrentUser(userId);

            var bookdto =  _userService.AddFavorite(userId, bookId);
            if (bookdto == null)
            {
                return Conflict($"Book {bookId} is already in favorites.");
            }
            return Ok(bookdto);
        }

        [HttpGet("{userId}/favorites")]
        public IActionResult GetFavorites(int userId)
        {
            EnsureCurrentUser(userId);

            var favorites = _userService.GetFavorites(userId);
            return Ok(favorites);
        }

        [HttpDelete("{userId}/favorites/{bookId}")]
        public IActionResult RemoveFavorite(int userId, int bookId)
        {
            EnsureCurrentUser(userId);

            bool flag = _userService.RemoveFavorite(userId, bookId);
            if (!flag)
            {
                return NotFound($"Book {bookId} not found in favorites.");
            }
            return Ok($"Book {bookId} removed from favorites.");
        }

        // Wishlist
        [HttpPost("{userId}/wishlist/{bookId}")]
        public IActionResult AddWishlist(int userId, int bookId)
        {
            EnsureCurrentUser(userId);

            var bookdto=  _userService.AddToWishlist(userId, bookId);
            if (bookdto == null)
            {
                return Conflict($"Book {bookId} is already in wishlist.");
            }
            return Ok(bookdto);
        }

        [HttpGet("{userId}/wishlist")]
        public IActionResult GetWishlist(int userId)
        {
            EnsureCurrentUser(userId);

            var wishlist = _userService.GetWishlist(userId);
            return Ok(wishlist);
        }

        [HttpDelete("{userId}/wishlist/{bookId}")]
        public IActionResult RemoveWishlist(int userId, int bookId)
        {
            EnsureCurrentUser(userId);

            bool flag = _userService.RemoveFromWishlist(userId, bookId);
            if (!flag)
            {
                return NotFound($"Book {bookId} not found in wishlist.");
            }
            return Ok($"Book {bookId} removed from wishlist.");
        }

        // Move from wishlist to favorites
        [HttpPost("{userId}/wishlist/{bookId}/move-to-favorites")]
        public IActionResult MoveWishlistToFavorites(int userId, int bookId)
        {
            EnsureCurrentUser(userId);

            bool flag = _userService.MoveWishlistToFavorites(userId, bookId);
            if (!flag)
            {
                return Ok($"Book {bookId} is already in favorites.");
            }
            return Ok($"Book {bookId} moved from wishlist to favorites.");
        }
    }
}
