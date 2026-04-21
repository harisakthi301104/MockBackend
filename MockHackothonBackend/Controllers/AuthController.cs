using Microsoft.AspNetCore.Mvc;
using MockHackothonBackend.DTOs.Auth;
using MockHackothonBackend.Services;


namespace MockHackothonBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.Register(dto);
            
            if (result == null)
                return BadRequest(new { error = "Email already exists or invalid role" });

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.Login(dto);
            
            if (result == null)
                return Unauthorized(new { error = "Invalid email or password" });

            return Ok(result);
        }
    }
}
