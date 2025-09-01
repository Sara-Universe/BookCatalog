using RestApiProject.Models;
using System.Globalization;

namespace RestApiProject.Services
{
    public class CsvBorrowService
    {
        private readonly string _filePath;
        private readonly ILogger<CsvBorrowService> _logger;

        public CsvBorrowService(string filePath , ILogger<CsvBorrowService> logger)
        {
            _filePath = filePath;

            if (!File.Exists(_filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
                File.WriteAllText(_filePath, "UserId,BookId,Action,Timestamp\n");
            }
            _logger = logger;
        }

        public List<BorrowRecord> LoadBorrowHistory()
        {
            var records = new List<BorrowRecord>();

            foreach (var line in File.ReadAllLines(_filePath).Skip(1)) // skip header
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(',');

                records.Add(new BorrowRecord
                {
                    UserId = int.Parse(parts[0]),
                    BookId = int.Parse(parts[1]),
                    Action = parts[2],
                    Timestamp = DateTime.Parse(parts[3], CultureInfo.InvariantCulture)
                });
            }

            return records;
        }

        public void AddBorrowRecord(BorrowRecord record)
        {
            var line = $"{record.UserId},{record.BookId},{record.Action},{record.Timestamp:o}";
            File.AppendAllText(_filePath, line + Environment.NewLine);
        }
    }
}
