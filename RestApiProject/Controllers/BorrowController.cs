using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestApiProject.Services;
using System.Security.Claims;

namespace RestApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BorrowController : ControllerBase
    {
        private readonly BorrowService _borrowService;

        public BorrowController(BorrowService borrowService)
        {
            _borrowService = borrowService;
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

        [HttpPost("books/{bookId}/borrow")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult BorrowBook(int bookId)
        {
            if (bookId < 1)
                return BadRequest(new { error = "Invalid book ID" });

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var success = _borrowService.BorrowBook(userId, bookId);

            if (!success)
                return Conflict(new { error = "Book is already borrowed" });

            return Ok(new { message = "Book borrowed successfully" });
        }

        [HttpPost("books/{bookId}/return")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult ReturnBook(int bookId)
        {
            if (bookId < 1)
                return BadRequest(new { error = "Invalid book ID" });
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            _borrowService.ReturnBook(userId, bookId);

            return Ok(new { message = "Book returned successfully" });
        }

        [HttpGet("users/{userId}/history")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult GetUserHistory(int userId)
        {
            if(userId < 1)
                return BadRequest(new { error = "Invalid user ID" });
            EnsureCurrentUser(userId);

            return Ok(_borrowService.GetUserHistory(userId));
        }

        
        [HttpGet("books/{bookId}/history")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetBookHistory(int bookId)
        {
            if(bookId < 1)
                return BadRequest(new { error = "Invalid book ID" });
            return Ok(_borrowService.GetBookHistory(bookId));
        }

        [HttpGet("users/{userId}/borrowed")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult GetBorrowedBooksByUser(int userId)
        {
            if(userId < 1)
                return BadRequest(new { error = "Invalid user ID" });

            EnsureCurrentUser(userId);

            return Ok(_borrowService.GetBorrowedBooksByUser(userId));
        }

        [HttpGet("overdue")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllOverdueBooks()
        {
            var overdueBooks = _borrowService.GetAllOverdueBooks();
            return Ok(overdueBooks);
        }
    }
}