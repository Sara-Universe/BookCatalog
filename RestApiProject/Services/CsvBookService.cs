using AutoMapper;
using CsvHelper;
using RestApiProject.DTOs;
using RestApiProject.Models;
using System.Globalization;

namespace RestApiProject.Services
{
    public class CsvBookService
    {
        private readonly string _filePath;
        private readonly IMapper _mapper;
        private readonly ILogger<CsvBookService> _logger;


        public CsvBookService(IMapper mapper, string filePath, ILogger<CsvBookService> logger)
        {
            _filePath = filePath;
            _mapper = mapper;
            _logger = logger;
        }
        public List<Book> LoadBooks()
       
        {
            if (!File.Exists(_filePath))
            {
                _logger.LogInformation($"Book file not found at path: {_filePath}");

                throw new FileNotFoundException($"Book file not found at path: {_filePath}");
            }
                var fileInfo = new FileInfo(_filePath);
                if (fileInfo.Length == 0)
                { 
                    // Empty file
                    return new List<Book>();
                }
            using var reader = new StreamReader(_filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // Reads all rows into books and return a list of books
            var bookdto = csv.GetRecords<BookCreationDto>().ToList();
            var book = _mapper.Map<List<Book>>(bookdto);
            _logger.LogInformation("Book Catalog is loaded");

            return book;
                
            }
        
        public void AddBook(Book newBook)
        {
            bool fileExists = File.Exists(_filePath);
            bool hasHeader = fileExists && new FileInfo(_filePath).Length > 0;

            using var writer = new StreamWriter(_filePath, append: true);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            if (!hasHeader)
            {
                csv.WriteHeader<Book>();
                csv.NextRecord();
            }

            csv.WriteRecord(newBook);
            csv.NextRecord();
        }


        public void UpdateBook(Book updatedBook)
        {
            var tempFile = Path.GetTempFileName();

            using (var reader = new StreamReader(_filePath))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            using (var writer = new StreamWriter(tempFile))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                var records = csvReader.GetRecords<Book>().ToList();

                foreach (var record in records)
                {
                    if (record.BookID == updatedBook.BookID)
                    {
                        record.Title = updatedBook.Title;
                        record.Author = updatedBook.Author;
                        record.Genre = updatedBook.Genre;
                        record.PublishedYear= updatedBook.PublishedYear;
                        record.Price = updatedBook.Price;
                    }
                }

                csvWriter.WriteRecords(records);
            }

            File.Delete(_filePath);
            File.Move(tempFile, _filePath, true);
        }

        // Delete book by ID
        public void DeleteBook(int id)
        {
            var tempFile = Path.GetTempFileName();

            using (var reader = new StreamReader(_filePath))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            using (var writer = new StreamWriter(tempFile))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                var records = csvReader.GetRecords<Book>().Where(b => b.BookID != id).ToList();
                csvWriter.WriteRecords(records);
            }

            File.Delete(_filePath);
            File.Move(tempFile, _filePath, true);
        }

    }
}
