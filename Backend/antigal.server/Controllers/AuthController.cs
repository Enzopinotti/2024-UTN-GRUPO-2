using Microsoft.AspNetCore.Mvc;
using antigal.server.Models.Dto;
using antigal.server.Services;
using System.Threading.Tasks;
using antigal.server.Models;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<User> _userManager;
        private readonly ServiceToken _serviceToken;

        public AuthController(IAuthService authService, UserManager<User> userManager, ServiceToken serviceToken)
        {
            _authService = authService;
            _userManager = userManager;
            _serviceToken = serviceToken;
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

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            var principal = _serviceToken.GetPrincipalFromExpiredToken(token); // Llama el método desde ServiceToken
            if (principal == null)
            {
                return BadRequest("Token inválido.");
            }

            var userId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var user = await _authService.GetUserByIdAsync(userId); // Asegúrate de tener este método en tu servicio
            if (user == null)
            {
                return BadRequest("Usuario no encontrado.");
            }

            // Confirmar el correo electrónico
            var result = await _authService.ConfirmEmailAsync(userId, token); // Cambia el método de acuerdo a tu implementación
            if (result)
            {
                return Ok("Email confirmado con éxito.");
            }

            return BadRequest("Error al confirmar el correo electrónico.");
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
