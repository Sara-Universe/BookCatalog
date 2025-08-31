using AutoMapper;
using CsvHelper;
using RestApiProject.DTOs;
using RestApiProject.Models;
using System.Globalization;

namespace RestApiProject.Services
{
    public class CsvUserService
    {
        private readonly string _filePath;
        private readonly IMapper _mapper;
        private readonly ILogger<CsvUserService> _logger;

        public CsvUserService(IMapper mapper, string filePath, ILogger<CsvUserService> logger)
        {
            _filePath = filePath;
            _mapper = mapper;
            _logger = logger;
        }

        // Load users from CSV
        public List<User> LoadUsers()
        {
            if (!File.Exists(_filePath))
            {
                _logger.LogWarning($"User file not found at path: {_filePath}");
                return new List<User>();
            }

            var fileInfo = new FileInfo(_filePath);
            if (fileInfo.Length == 0)
            {
                _logger.LogWarning("User file is empty.");
                return new List<User>();
            }

            using var reader = new StreamReader(_filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            // Register list<int> converter
            csv.Context.TypeConverterCache.AddConverter<List<int>>(new IntListConverter());

            // ⬇️ Directly load into User, not DTO
            var users = csv.GetRecords<User>().ToList();

            _logger.LogInformation("Users loaded successfully from CSV.");
            return users;
        }




        // Save all users back to CSV
        public void SaveUsers(List<User> users)
        {
            using var writer = new StreamWriter(_filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.Context.TypeConverterCache.AddConverter<List<int>>(new IntListConverter());

            csv.WriteHeader<User>();
            csv.NextRecord();
            csv.WriteRecords(users);
        }
    }

    // Converter for List<int>
    public class IntListConverter : CsvHelper.TypeConversion.DefaultTypeConverter
    {
        public override object ConvertFromString(string text, CsvHelper.IReaderRow row, CsvHelper.Configuration.MemberMapData memberMapData)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<int>();

            return text.Split(';').Select(int.Parse).ToList();
        }

        public override string ConvertToString(object value, CsvHelper.IWriterRow row, CsvHelper.Configuration.MemberMapData memberMapData)
        {
            if (value is List<int> list && list.Any())
                return string.Join(";", list);

            return string.Empty;
        }
    }
}
