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

        public AuthController(JWTService jwtService, UserService userServ)
        {
            _jwtService = jwtService;
            _userServ = userServ;
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] UserDto request)
        {
            var user = _userServ.FindByUsername(request.Username);

            if (user != null && _userServ.CheckPassword(user, request.Password))
            {
                var token = _jwtService.GenerateToken(user);
                return Ok(new { token });
            }

            return Unauthorized("Invalid username or password");
        }
    }
}
