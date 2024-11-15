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
using System.Text;

namespace antigal.server.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly JwtHandler _jwtHandler;
        private readonly IEmailSender _emailSender;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IMapper mapper,
            JwtHandler jwtHandler,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _jwtHandler = jwtHandler;
            _emailSender = emailSender;
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

            // Generar y codificar el token de confirmación de email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            // Configurar URL de confirmación
            var clientUri = !string.IsNullOrWhiteSpace(userForRegistration.ClientUri)
                ? userForRegistration.ClientUri
                : "http://localhost:7255/authentication/confirm-email";

            var callback = $"{clientUri}?userId={user.Id}&token={encodedToken}";

            // Enviar email de confirmación
            var message = new Message(new string[] { user.Email! }, "Email Confirmation",
                $"<p>Por favor confirma tu cuenta haciendo clic en el siguiente enlace: <a href='{callback}'>Confirmar email</a></p>");

            await _emailSender.SendEmailAsync(message);
            await _userManager.AddToRoleAsync(user, "User");

            return StatusCode(201, new { Message = "Usuario registrado exitosamente. Revisa tu email para confirmar tu cuenta." });
        }

        // Método de autenticación inicial
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

            // Generar y guardar el refresh token con duración de 30 minutos
            var refreshToken = _jwtHandler.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(30); // Expira en 30 minutos
            await _userManager.UpdateAsync(user);

            return Ok(new AuthResponseDto { IsAuthSuccesful = true, Token = token, RefreshToken = refreshToken });
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

            // Decodificar el token antes de confirmar
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
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

            // Generar y codificar el token de restablecimiento de contraseña
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            // Crear enlace de restablecimiento de contraseña
            var callback = $"{forgotPassword.ClientUri}?token={encodedToken}&email={forgotPassword.Email}";
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

            // Decodificar el token antes de usarlo
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPassword.Token!));
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPassword.Password!);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }
            return Ok();
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenDto tokenDto)
        {
            if (tokenDto == null || string.IsNullOrEmpty(tokenDto.Token))
            {
                return BadRequest(new AuthResponseDto
                {
                    IsAuthSuccesful = false,
                    ErrorMessage = "Invalid client request"
                });
            }

            var principal = _jwtHandler.GetPrincipalFromExpiredToken(tokenDto.Token);
            if (principal == null)
            {
                return BadRequest(new AuthResponseDto
                {
                    IsAuthSuccesful = false,
                    ErrorMessage = "Invalid access token or refresh token"
                });
            }

            var username = principal.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest(new AuthResponseDto
                {
                    IsAuthSuccesful = false,
                    ErrorMessage = "Invalid access token or refresh token"
                });
            }

            var user = await _userManager.FindByNameAsync(username);

            if (user == null || string.IsNullOrEmpty(user.RefreshToken) || user.RefreshTokenExpiryTime <= DateTime.Now || user.RefreshToken != tokenDto.RefreshToken)
            {
                return Unauthorized(new AuthResponseDto
                {
                    IsAuthSuccesful = false,
                    ErrorMessage = "Invalid or expired refresh token"
                });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var newAccessToken = _jwtHandler.CreateToken(user, roles);
            var newRefreshToken = _jwtHandler.GenerateRefreshToken();

            // Actualizar el usuario con el nuevo refresh token con duración de 30 minutos
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(30); // Nueva expiración de 30 minutos
            await _userManager.UpdateAsync(user);

            return Ok(new AuthResponseDto
            {
                IsAuthSuccesful = true,
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }




    }
}
