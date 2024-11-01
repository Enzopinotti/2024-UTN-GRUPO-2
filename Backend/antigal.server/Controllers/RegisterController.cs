using Microsoft.AspNetCore.Mvc;
using antigal.server.Models.Dto;
using antigal.server.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using antigal.server.JwtFeatures;
using antigal.server.Services;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly JwtHandler _jwtHandler;
        private readonly IEmailSender _emailSender; // Servicio de email agregado

        public RegisterController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IMapper mapper,
            JwtHandler jwtHandler,
            IEmailSender emailSender) // Constructor modificado
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _jwtHandler = jwtHandler;
            _emailSender = emailSender; // Inicialización del servicio de email
        }

        // Registro de usuario regular (rol predeterminado: "User")
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto)
        {
            if (registerDto is null)
            {
                return BadRequest();
            }

            var user = _mapper.Map<User>(registerDto);
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new RegisterResponseDto { Errors = errors });
            }

            await _userManager.AddToRoleAsync(user, "User");

            // Generar el token de confirmación de email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Register", new { userId = user.Id, token }, Request.Scheme);

            // Verificar si el email no es nulo antes de enviar el correo
            if (string.IsNullOrEmpty(user.Email))
            {
                return BadRequest("No se pudo enviar el correo de confirmación porque el email es nulo.");
            }

            await _emailSender.SendEmailAsync(user.Email, "Confirma tu email", $"Por favor confirma tu cuenta haciendo clic en el enlace: <a href='{confirmationLink}'>Confirmar email</a>");

            return Ok(new { Message = "Usuario registrado exitosamente. Por favor verifica tu email para confirmar tu cuenta." });
        }

        // Método para confirmar el email
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Datos inválidos.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok(new { Message = "Email confirmado exitosamente. Ahora puedes iniciar sesión." });
            }

            return BadRequest("Error al confirmar el email.");
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UseForAuthenticationDto authenticationResponse)
        {
            var user = await _userManager.FindByNameAsync(authenticationResponse.Email!);
            if (user is null || !await _userManager.CheckPasswordAsync(user, authenticationResponse.Password!))
                return Unauthorized(new AuthenticationResponseDto { ErrorMessage = "Autenticación inválida." });

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized(new AuthenticationResponseDto { ErrorMessage = "El email no fue confirmado." });

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtHandler.CreateToken(user, roles);
            return Ok(new AuthenticationResponseDto { IsAuthSuccesful = true, Token = token });
        }
    }
}
