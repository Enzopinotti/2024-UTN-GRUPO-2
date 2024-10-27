using Microsoft.AspNetCore.Mvc;
using antigal.server.Models.Dto;
using antigal.server.Services;
using System.Threading.Tasks;
using antigal.server.Models;

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

            return Ok("Usuario reegistrado con exito!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tokens = await _authService.LoginUserAsync(loginDto);
            if (tokens.AccessToken == null || tokens.RefreshToken == null)
            {
                return Unauthorized();
            }

            return Ok(new { AccessToken = tokens.AccessToken, RefreshToken = tokens.RefreshToken });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenDto refreshTokenDto)
        {
            if (string.IsNullOrEmpty(refreshTokenDto.RefreshToken))
            {
                return BadRequest("Refresh token no puede estar vacío.");
            }

            var newAccessToken = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
            if (string.IsNullOrEmpty(newAccessToken))
            {
                return Unauthorized();
            }

            return Ok(new { AccessToken = newAccessToken });
        }
    }
}
