using Microsoft.AspNetCore.Mvc;
using PollApp.Backend.DTOs;
using PollApp.Backend.Services;
using System.Threading.Tasks;

namespace PollApp.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var response = await _authService.RegisterAsync(dto);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var response = await _authService.LoginAsync(dto);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var response = await _authService.RefreshTokenAsync(dto);
            return Ok(response);
        }
    }
}
