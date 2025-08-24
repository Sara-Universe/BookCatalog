using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestApiProject.DTOs;
using RestApiProject.Services;

namespace RestApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]//  all endpoints require authentication
    public class BookController : ControllerBase
    {

        private readonly BookService _bookService;
        private readonly IMapper _mapper;




        public BookController(BookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }

       
        //default value if the user didnot specify the page number and size it will show page 1 with 10 books


        [HttpGet]
        public IActionResult GetfromQuery([FromQuery] string? author , [FromQuery] string? genre,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {

            if ( string.IsNullOrEmpty(author) && string.IsNullOrEmpty(genre))
            {
                return Ok(_bookService.GetBooks(pageNumber, pageSize));
            }
            
           var result = _bookService.GetByAuthor_genre(author, genre);

            if ( result == null || !result.Any())
            {
                return NotFound("there are no books with these criteria");
            }
            return Ok(result);
        }

        [HttpGet ("{id}")]
        public IActionResult GetByID (int id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                return NotFound($"There is no book with id {id}");
            }
            return Ok(book);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post([FromBody] BookDTO book)
        {
            if (book == null)
                return BadRequest("Book body is required.");

            var (isSuccess, errors,createdBook) = _bookService.AddBook(book);

            if (!isSuccess)
                return BadRequest(new { Errors = errors });
            return CreatedAtAction(nameof(GetByID), new { id = createdBook.BookID }, createdBook);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Put(int id, [FromBody] BookDTO bookdto)
        {
            if (bookdto == null)
                return BadRequest("Book body is required.");

            var (isSuccess, errors, updatedBook) = _bookService.UpdateBook(id, bookdto);

            if (!isSuccess)
                return BadRequest(new { Errors = errors });
            return Ok(updatedBook);
        }

        [HttpDelete ("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
    
            var deletedbook = _bookService.DeletById(id);
            if(deletedbook == false)
            {
                return NotFound($"There is no book to delete with id {id}");
            }


            return Ok("The book has been deleted successfully!");
        }

        [HttpGet("search")]

        public IActionResult SearchbyTitle ([FromQuery] string? keyword)
        {
            if (keyword == null)
            {
                return BadRequest("You should enter a keyword");

            }
            var result = _bookService.searchByTitle(keyword);
            if(result == null)
            {
                return NotFound($"There is no title that contains the keyword ({keyword})");
            }


            return Ok(result);
        }

    }
}
