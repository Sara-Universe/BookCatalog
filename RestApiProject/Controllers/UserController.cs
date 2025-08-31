using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestApiProject.Services;
using System.Security.Claims;

namespace RestApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require authentication
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
            // JWT should include the user ID as a claim
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User ID claim missing in token.");

            return int.Parse(userIdClaim);
        }

        private void EnsureCurrentUser(int userId)
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId != userId)
            {
                throw new UnauthorizedAccessException("You cannot modify another user's list.");
            }
        }

        // Favorites
        [HttpPost("{userId}/favorites/{bookId}")]
        public IActionResult AddFavorite(int userId, int bookId)
        {
            EnsureCurrentUser(userId);

            _userService.AddFavorite(userId, bookId);
            return Ok($"Book {bookId} added to favorites.");
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

            _userService.RemoveFavorite(userId, bookId);
            return Ok($"Book {bookId} removed from favorites.");
        }

        // Wishlist
        [HttpPost("{userId}/wishlist/{bookId}")]
        public IActionResult AddWishlist(int userId, int bookId)
        {
            EnsureCurrentUser(userId);

            _userService.AddToWishlist(userId, bookId);
            return Ok($"Book {bookId} added to wishlist.");
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

            _userService.RemoveFromWishlist(userId, bookId);
            return Ok($"Book {bookId} removed from wishlist.");
        }

        // Move from wishlist to favorites
        [HttpPost("{userId}/wishlist/{bookId}/move-to-favorites")]
        public IActionResult MoveWishlistToFavorites(int userId, int bookId)
        {
            EnsureCurrentUser(userId);

            _userService.MoveWishlistToFavorites(userId, bookId);
            return Ok($"Book {bookId} moved from wishlist to favorites.");
        }
    }
}
