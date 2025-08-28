using AutoMapper;
using RestApiProject.Models;

namespace RestApiProject.Services
{
    public class UserService
    {
        private readonly ILogger<UserService> _logger;


        public UserService(ILogger<UserService> logger)
        {
          
            _logger = logger;
        }
        // This is your in-memory user storage
        private readonly List<User> _users = new()
    {
        new User { Username = "admin", Password = "admin123", Role = "Admin" },
        new User { Username = "sara", Password = "user123", Role = "User" }
    };

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
                _logger.LogInformation($"Unauthorized:\n\tInvalid password");
                return false;

            }

            return true;
        }
    }
}
