using Microsoft.AspNetCore.Http;
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
        private readonly UserService _userServ;
        private readonly ILogger<BookService> _logger;


        public AuthController(JWTService jwtService, UserService userServ, ILogger<BookService> logger)
        {
            _jwtService = jwtService;
            _userServ = userServ;
            _logger = logger;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDto request)
        {
            var user = _userServ.FindByUsername(request.Username);

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
