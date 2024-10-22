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

            // asignar token
            //var token = await _tokenService.CreateToken(user);

            return Ok("Usuario reegistrado con exito!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var token = await _authService.LoginUserAsync(loginDto);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            return Ok(new { Token = token });
        }
    }
}
