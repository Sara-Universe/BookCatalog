using Microsoft.AspNetCore.Mvc;
using RestApiProject.DTOs;
using RestApiProject.Services;

namespace RestApiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly JWTService _jwtService;
        private readonly UserLoginService _userServ;
        private readonly ILogger<BookController> _logger;

        public AuthController(JWTService jwtService, UserLoginService userServ, ILogger<BookController> logger)
        {
            _jwtService = jwtService;
            _userServ = userServ;
            _logger = logger;

        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            var user = _userServ.FindByUsername(request.Username);

            _logger.LogInformation($"Login attempt for user: {request.Username}");
            if (user == null)
            {
                _logger.LogInformation($"Unauthorized:\n\tThe username is not found!!");
            }
            if (user != null && _userServ.CheckPassword(user, request.Password))
            {
                var token = _jwtService.GenerateToken(user);
                _logger.LogInformation($"You have logged in as: {user.Role}");
                return Ok(new { token });
            }

            return Unauthorized("Invalid username or password");
        }
    }
}
