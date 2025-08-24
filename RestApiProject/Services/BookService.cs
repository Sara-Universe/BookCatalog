using AutoMapper;
using CsvHelper;
using RestApiProject.Controllers;
using RestApiProject.DTOs;
using RestApiProject.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace RestApiProject.Services
{
    public class BookService
    {
        private readonly IMapper _mapper;
        private readonly string  _filePath;
        private readonly List<Book> _books; // in memory storage
        private readonly ILogger<BookService> _logger;



        public BookService ( IMapper mapper, string filePath)
        {
            _mapper = mapper;
            _filePath = filePath;
            _books = LoadBooks(_filePath);
         
        


        }

      
        public List<Book> LoadBooks(string filePath)
        {
            List<BookDTO> bookDtos;

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Reads all rows into DTOs
                bookDtos = csv.GetRecords<BookDTO>().ToList();
            }

            // Map DTOs to Book entities

            var books = _mapper.Map<List<Book>>(bookDtos);

            //_logger.LogInformation("Book Catalog is loaded");


            return books;
        }


        public PagedResult<BookDTO> GetBooks(int pageNumber, int pageSize)
        {
            var totalCountOfBooks = _books.Count;
            //do the ordering before the paging
            var sortedByYear = _books.OrderBy(x => x.PublishedYear);

            var books = sortedByYear
              .Skip((pageNumber - 1) * pageSize)
              .Take(pageSize)
              .ToList();


            var booksto =  _mapper.Map<List<BookDTO>>(books);
            //_logger.LogInformation($"Get All Books -> Country: {booksto.Count()}");

            return new PagedResult<BookDTO>
            {
                Items = booksto,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCountOfBooks
            };
        }

        public BookDTO? GetBookById(int id)
        {
            var book = _books.FirstOrDefault(i => i.BookID == id);
            if (book == null)
            {
                _logger.LogInformation($"Book with id: {id} is not found");

                return null;

            }

            //_logger.LogInformation($"Get Book with id {book.BookID} : Title {book.Title}");

            return _mapper.Map<BookDTO>(book);
        }



        public IEnumerable<BookDTO>? GetByAuthor_genre (string? author ,  string? genre)
        {
                var books = _books.AsEnumerable();// initi
   
               
            if (!string.IsNullOrEmpty(author))
            {
                  books = books.Where(i => i.Author == author).OrderBy(x=> x.Genre);

           
            }
            if (!string.IsNullOrEmpty(genre)){
                books = books.Where(i=> i.Genre == genre).OrderBy(x=> x.Price);
            }
            if(books == null)
            {
                _logger.LogInformation($"Book/List of books with this author, genre or both are not founds ");

                return null;
            }

            var list  =books.ToList();
            //_logger.LogInformation($"Get Book/Books that belongs to this author {author} and/or genre {genre} -> their counts are {list.Count()}");

            return _mapper.Map<List<BookDTO>>(list);
            }

        public (bool IsSuccess, List<string> Errors, BookDTO CreatedBook) AddBook (BookDTO bookdto)
        {
            var context = new ValidationContext(bookdto, null, null);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(bookdto, context, results, true);

            if (!isValid)
            {
                // return all the error messages
                var errors = results.Select(r => r.ErrorMessage ?? "Invalid field").ToList();
               // _logger.LogInformation($"");

                return (false,errors,null);
            }
            int newId = (_books.Any()) ? _books.Max(b => b.BookID) + 1 : 1;

            //map dto to book so we add the new data to the origional list and file
            var book = _mapper.Map<Book>(bookdto);
            book.BookID =newId;
            _books.Add(book);//add to list
            AppendBookToCsv(book);//add to csv
            var dtoResult = _mapper.Map<BookDTO>(book);// converted again becuse we want to display it to client

            return (true, new List<string>(), dtoResult);

        }
     

        public (bool IsSuccess, List<string> Errors, BookDTO UpdatedBook) UpdateBook(int id, BookDTO bookdto)
        {

            var existingBook = _books.FirstOrDefault(i => i.BookID == id);
            if (existingBook == null)
            {
                    return (false, new List<string> { $"There is no Book with ID {id}." }, null);

            }
            var context = new ValidationContext(bookdto, null, null);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(bookdto, context, results, true);

            if (!isValid)
            {
                // return all the error messages
                var errors = results.Select(r => r.ErrorMessage ?? "Invalid field").ToList();
                return (false, errors, null);
            }

            // copies all matching properties from bookdto to existingBook
            _mapper.Map(bookdto, existingBook);
            existingBook.BookID = id; // to ensure that ID is not overwritten
            var dtoResult = _mapper.Map<BookDTO>(existingBook);

            

            return (true, new List<string>(), dtoResult);
        }

        public bool DeletById (int id)
        {
            var deletedbook = _books.FirstOrDefault (i => i.BookID == id);
            if (deletedbook == null)
            {
                return false;
            }

            _books.Remove(deletedbook);
            RewriteCsvFile();
            return true;


        }


        public List<BookDTO>? searchByTitle(string keyword)
        {
         
            var result = _books.Where(i=> i.Title.Contains(keyword,StringComparison.OrdinalIgnoreCase)).ToList();
            if (!result.Any())
            {
                return null;
            }

            return _mapper.Map<List<BookDTO>>(result);
        }

        /// <CSV Helper>
        /// //////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        //rewrite entire CSV from the _books list
        private void RewriteCsvFile()
        {
            using var writer = new StreamWriter(_filePath, false); // overwrite
            using var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(_books);
        }
        private void AppendBookToCsv(Book book)
        {
            var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false // don’t write header when appending
            };

            using var writer = new StreamWriter(_filePath, append: true);
            using var csv = new CsvWriter(writer, config);

            csv.WriteRecord(book); // writes the properties in order
            csv.NextRecord();      // moves to next line
        }

    }//end of class
}
