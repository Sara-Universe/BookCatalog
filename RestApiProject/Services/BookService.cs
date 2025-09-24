using AutoMapper;
using CsvHelper;

using RestApiProject.DTOs;
using RestApiProject.Exceptions;
using RestApiProject.Models;
using RestApiProject.Services;
using System.Globalization;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RestApiProject.Services
{
    public class BookService
    {
        private readonly CsvBookService _csvService;
        private readonly List<Book> _books; // in memory storage
        private readonly ILogger<BookService> _logger;
        private readonly IMapper _mapper;

        public BookService(CsvBookService csvService, IMapper mapper, ILogger<BookService> logger)
        {
            _csvService = csvService;      
            _mapper = mapper;    
            _logger = logger;
            _books = _csvService.LoadBooks(); 

        }


  
        public BookCreationDto GetBookById(int id)
        {
            var book = _books.FirstOrDefault(i => i.Id == id);
            if (book == null)
            {
                 _logger.LogInformation($"Book with id: {id} is not found");

                throw new NotFoundException($"Book with id {id} was not found.");


            }
             
            _logger.LogInformation($"Get Book with id {book.Id} : Title {book.Title}");

            return _mapper.Map<BookCreationDto>(book);
        }



        public IEnumerable<BookCreationDto> GetBooksFiltered(string? author, string? genre, string? keyword)
        {

            if (_books == null || !_books.Any())
            {
                _logger.LogInformation($"No books are available in the catalog");

                throw new NotFoundException("No books are available in the catalog.");
            }
            var books = _books.AsEnumerable();// initi

            if (!string.IsNullOrEmpty(author))
            {
                books = books.Where(i => i.Author.Equals(author, StringComparison.OrdinalIgnoreCase));

            }
            if (!string.IsNullOrEmpty(genre))
            {
                books = books.Where(i => i.Genre.Equals(genre, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                books = books.Where(i => i.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }
            if (books.Count() == _books.Count())
                _logger.LogInformation($"Get all books\nthe number of returned books are: {books.Count()}");
            else
                _logger.LogInformation($"Get books based on author: {author}\ngenre: {genre}\nkeyword: {keyword}\nthe number of returned books are: {books.Count()}");

            var result = _mapper.Map<List<BookCreationDto>>(books);
            return result.OrderBy(i => i.PublishedYear);
        }

        public BookCreationDto AddBook(BookDTO bookdto)
        {
            if (_books == null)
            {
                _logger.LogInformation($"Book list is not initialized");

                throw new Exception("Book list is not initialized.");
            }
            int newId = (_books.Any()) ? _books.Max(b => b.Id) + 1 : 1;
            var book = _mapper.Map<Book>(bookdto);
            book.Id = newId;
            _books.Add(book);//add to list
            _csvService.AddBook(book);
            _logger.LogInformation($"New Book has been added with id {newId}");

            return _mapper.Map<BookCreationDto>(book);
        }


        public BookCreationDto UpdateBook(int id, BookDTO bookdto)
        {

            var existingBook = _books.FirstOrDefault(i => i.Id == id);
            if (existingBook == null) { 

                _logger.LogInformation($"There is no Book with ID {id}");
            
                throw new NotFoundException($"There is no Book with ID {id}.");
            }
             // Copy DTO properties to existing book
            _mapper.Map(bookdto, existingBook);
            existingBook.Id = id; // to ensure that ID is not overwritten
            _csvService.UpdateBook(existingBook);
            var result = _mapper.Map<BookCreationDto>(existingBook);
            _logger.LogInformation($"Book with id {id} has beedn updated");

            return result;
        }

        public bool DeleteById(int id)
        {
            var deletedBook = _books.FirstOrDefault(i => i.Id == id);
            if (deletedBook == null)
            {
                _logger.LogInformation($"Book with ID {id} was not found");

                throw new NotFoundException($"Book with ID {id} was not found.");
            }
            _books.Remove(deletedBook);
           _csvService.DeleteBook(id);
            _logger.LogInformation($"Book with id {id} has been deleted");

            return true;
        }
    }//end of class
}
