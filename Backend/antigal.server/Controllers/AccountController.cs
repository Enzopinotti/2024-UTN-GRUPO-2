using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using antigal.server.Models.Dto;
using antigal.server.Services;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var result = await _authService.RegisterAsync(model);

            if (result.Succeeded)
            {
                return Ok("User  created successfully");
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest($"Failed to create user: {errors}");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var token = await _authService.LoginAsync(model);

            if (token != null)
            {
                return Ok(new { Token = token });
            }

            return Unauthorized("Invalid username or password");
        }
    }
}
