using Microsoft.AspNetCore.Mvc;
using antigal.server.Models.Dto;
using antigal.server.Services;
using System.Threading.Tasks;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterUserAsync(registerDto);
            if (!result)
            {
                return BadRequest("Error al registrarse");
            }

            return Ok("Usuario registrado con éxito!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Credenciales no proporcionadas");
            }

            var accessToken = await _authService.LoginUserAsync(loginDto);
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized();
            }

            var user = await _authService.ValidateUserAsync(accessToken);
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(new { Token = accessToken, User = user });
        }
    }
}