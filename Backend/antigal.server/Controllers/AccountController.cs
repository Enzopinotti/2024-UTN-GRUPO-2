using Microsoft.AspNetCore.Mvc;
using antigal.server.Models.Dto;
using antigal.server.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using antigal.server.JwtFeatures;
using antigal.server.Services;
using EmailService;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualBasic;
namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountControler : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly JwtHandler _jwtHandler;
        private readonly IEmailSender _emailSender; // Servicio de email agregado

        public AccountControler(
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

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration is null)
                return BadRequest();

            var user = _mapper.Map<User>(userForRegistration);
            var result = await _userManager.CreateAsync(user, userForRegistration.Password!);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new RegistrationResponseDto { Errors = errors });
            }

            // Generar el token de confirmación de email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var param = new Dictionary<string, string?>
    {
        { "token", token },
        { "userId", user.Id },  // Agregamos userId aquí
        { "email", user.Email }
    };

            // Configurar ClientUri con un valor predeterminado si está vacío
            var clientUri = !string.IsNullOrWhiteSpace(userForRegistration.ClientUri)
                ? userForRegistration.ClientUri
                : "http://localhost:7255/authentication/confirm-email";  // Puedes cambiar a backend si prefieres

            var callback = QueryHelpers.AddQueryString(clientUri, param);
            var message = new Message(new string[] { user.Email! }, "Email Confirmation",
                $"<p>Por favor confirma tu cuenta haciendo clic en el siguiente enlace: <a href='{callback}'>Confirmar email</a></p>");

            await _emailSender.SendEmailAsync(message);
            await _userManager.AddToRoleAsync(user, "User");

            return StatusCode(201, new { Message = "Usuario registrado exitosamente. Revisa tu email para confirmar tu cuenta." });
        }


        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto userForAuthentication)
        {
            var user = await _userManager.FindByNameAsync(userForAuthentication.Email!);

            if (user is null)
            {
                return BadRequest("Invalid request.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized(new AuthResponseDto { ErrorMessage = "El email no fue confirmado." });

            if (!await _userManager.CheckPasswordAsync(user, userForAuthentication.Password!))
                return Unauthorized(new AuthResponseDto { ErrorMessage = "Autenticación inválida." });

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtHandler.CreateToken(user, roles);
            return Ok(new AuthResponseDto { IsAuthSuccesful = true, Token = token });
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


        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPassword)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(forgotPassword.Email!);
            if (user == null)
                return BadRequest("Usuario no encontrado.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var param = new Dictionary<string, string?>
    {
        { "token", token },
        { "email", forgotPassword.Email }
    };

            // Configurar ClientUri con un valor predeterminado para restablecimiento de contraseña
            var clientUri = !string.IsNullOrWhiteSpace(forgotPassword.ClientUri)
                ? forgotPassword.ClientUri
                : "http://localhost:7255/resetpassword";  // Valor predeterminado para frontend (cambia si es backend)

            var callback = QueryHelpers.AddQueryString(clientUri, param);
            var message = new Message(new string[] { user.Email! }, "Password Reset",
                $"<p>Para restablecer tu contraseña, haz clic en el siguiente enlace: <a href='{callback}'>Restablecer contraseña</a></p>");

            await _emailSender.SendEmailAsync(message);
            return Ok(new { Message = "Enlace de restablecimiento de contraseña enviado. Revisa tu correo electrónico." });
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPassword)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(resetPassword.Email!);
            if (user is null)
                return BadRequest("Invalid request");

            var result = await _userManager.ResetPasswordAsync(user, resetPassword.Token!, resetPassword.Password!);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }
            return Ok();
        }

    }
}