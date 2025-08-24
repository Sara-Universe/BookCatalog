using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestApiProject.DTOs;
using CustomValidationException = RestApiProject.Exceptions.ValidationException;

using RestApiProject.Services;
using RestApiProject.Models;
using System.ComponentModel.DataAnnotations;

namespace RestApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]//  all endpoints require authentication
    public class BookController : ControllerBase
    {

        private readonly BookService _bookService;
    




        public BookController(BookService bookService)
        {
            _bookService = bookService;
        }

       
        //default value if the user didnot specify the page number and size it will show page 1 with 10 books


        [HttpGet]
        public IActionResult Filtering([FromQuery] string? author , 
            [FromQuery] string? genre, [FromQuery] string ? keyword , 
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1)
                throw new CustomValidationException("Page number must be greater than 0.");

            if (pageSize < 1)
                throw new CustomValidationException("Page size must be greater than 0.");

            var books = _bookService.GetBooksFiltered(author, genre , keyword);
            int totalCount = books.Count();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (totalPages > 0 && pageNumber > totalPages)
                throw new CustomValidationException($"Page number {pageNumber} exceeds the total pages {totalPages}.");
            // Apply paging
            var pagedItems = books
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            //Build PagedResult
            var pagedResult = new PagedResult<BookCreationDto>
            {
                Items = pagedItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = books.Count()
            };

            return Ok(pagedResult);
        }

        [HttpGet ("{id}")]
        public IActionResult GetByID (int id)
        {
            var book = _bookService.GetBookById(id);

            return Ok(book);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Add([FromBody] BookDTO bookdto)
        {
            if (bookdto == null)
                return BadRequest(new { Message = "Book body is required." });

            var context = new ValidationContext(bookdto, null, null);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(bookdto, context, results, true);

            if (!isValid)
            {
                var errors = results.Select(r => r.ErrorMessage ?? "Invalid field").ToList();
                return BadRequest(new { Errors = errors });
            }

            // Call service to create book
            var createdBook = _bookService.AddBook(bookdto); // throws exception if any problem
            return CreatedAtAction(nameof(GetByID), new { id = createdBook.BookID }, createdBook);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, [FromBody] BookDTO bookdto)
        {
            if (bookdto == null)
                return BadRequest(new { Message = "Book body is required." });

            var context = new ValidationContext(bookdto, null, null);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(bookdto, context, results, true);
            if (!isValid)
            {
                var errors = results.Select(r => r.ErrorMessage ?? "Invalid field").ToList();
                return BadRequest(new { Errors = errors });
            }

            var updatedBook = _bookService.UpdateBook(id, bookdto);

            return Ok(updatedBook);
        }

        [HttpDelete ("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            _bookService.DeleteById(id);
            return NoContent();
        }

    }
}
