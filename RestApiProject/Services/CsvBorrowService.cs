using CsvHelper;
using CsvHelper.Configuration;
using RestApiProject.Models;
using System.Globalization;

namespace RestApiProject.Services
{
    public class CsvBorrowService
    {
        private readonly string _filePath;
        private readonly ILogger<CsvBorrowService> _logger;

        public CsvBorrowService(string filePath, ILogger<CsvBorrowService> logger)
        {
            _filePath = filePath;
            _logger = logger;
        }

        // Load borrow history from CSV
        public List<BorrowRecord> LoadBorrowHistory()
        {
            if (!File.Exists(_filePath))
            {
                _logger.LogInformation($"Borrow history file not found at path: {_filePath}. Creating new one.");
                return new List<BorrowRecord>();
            }

            var fileInfo = new FileInfo(_filePath);
            if (fileInfo.Length == 0)
            {
                _logger.LogInformation("Borrow history file is empty.");
                return new List<BorrowRecord>();
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true, // expect headers if file isn’t empty
                MissingFieldFound = null, // ignore missing fields
                HeaderValidated = null    // ignore extra headers
            };

            using var reader = new StreamReader(_filePath);
            using var csv = new CsvReader(reader, config);

            var records = csv.GetRecords<BorrowRecord>().ToList();
            _logger.LogInformation($"Loaded {records.Count} borrow records from history.");
            return records;
        }
        public void AddBorrowRecord(BorrowRecord record)
        {
            bool fileExists = File.Exists(_filePath);
            bool isEmpty = !fileExists || new FileInfo(_filePath).Length == 0;

            if (isEmpty)
            {
                // First time: create the file, write header + record
                using var writer = new StreamWriter(_filePath, append: false);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                csv.WriteHeader<BorrowRecord>();
                csv.NextRecord();
                csv.WriteRecord(record);
                csv.NextRecord();
            }
            else
            {
                // Append only the record
                using var writer = new StreamWriter(_filePath, append: true);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                csv.WriteRecord(record);
                csv.NextRecord();
            }

            _logger.LogInformation(
                $"Borrow record added: User {record.UserId}, Book {record.BookId}, Action {record.Action}, Timestamp {record.Timestamp}"
            );
        }


    }
}
