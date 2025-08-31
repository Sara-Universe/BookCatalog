using AutoMapper;
using RestApiProject.Models;

namespace RestApiProject.Services
{
    public class UserLoginService
    {
        private readonly ILogger<UserLoginService> _logger;
        private readonly CsvUserService _csvUserService;
        private readonly List<User> _users;

        public UserLoginService(ILogger<UserLoginService> logger, CsvUserService csvUserService)
        {
            _logger = logger;
            _csvUserService = csvUserService;

            // Load all users once when service is created
            _users = _csvUserService.LoadUsers();
      
        }

        

        // Find a user by username
        public User? FindByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
        }

        // Check if the username and  password match
        public bool CheckPassword(User user, string password)
        {
            if (user == null)
                 return false;
            
         if (user.Password != password)
            {
                _logger.LogInformation($"password : {user.Password}");
                _logger.LogInformation($"Unauthorized:\n\tInvalid password");
                return false;

            }

            return true;
        }
    }
}
