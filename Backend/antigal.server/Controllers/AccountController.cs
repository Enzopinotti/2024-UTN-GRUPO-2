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

        // Registro de usuario regular (rol predeterminado: "User")
   [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UseForAuthenticationDto useForAuthentication)
        {
            if (useForAuthentication is null)
            {
                return BadRequest();
            }

            var user = _mapper.Map<User>(useForAuthentication);
            var result = await _userManager.CreateAsync(user, useForAuthentication.Password!);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new RegisterResponseDto { Errors = errors });
            }

    
            // Generar el token de confirmación de email
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var param = new Dictionary<string, string?>
            {
                { "token", token },
                { "email", user.Email }
            };
            var callback = QueryHelpers.AddQueryString(useForAuthentication.ClientUri!, param);
            var message = new Message([user.Email!], "Email confirmation token", callback, null);

            await _emailSender.SendEmailAsync(message);
            await _userManager.AddToRoleAsync(user, "User");
            return StatusCode(201);
            /*
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Register", new { userId = user.Id, token }, Request.Scheme);

            // Verificar si el email no es nulo antes de enviar el correo
            if (string.IsNullOrEmpty(user.Email))
            {
                return BadRequest("No se pudo enviar el correo de confirmación porque el email es nulo.");
            }

            // Enviar correo de confirmación utilizando el servicio de EmailSender
            // Suponiendo que la función en el servicio de correo sea `SendEmail` (ajustalo según corresponda)
            var emailSubject = "Confirma tu email";
            var emailBody = $"Por favor confirma tu cuenta haciendo clic en el enlace: <a href='{confirmationLink}'>Confirmar email</a>";

            var sendEmailResult = await _emailSender.SendEmailAsync(user.Email, emailSubject, emailBody); // Ajusta esta llamada

            if (!sendEmailResult)
            {
                return BadRequest("Hubo un problema al enviar el correo de confirmación.");
            }

            return Ok(new { Message = "Usuario registrado exitosamente. Por favor verifica tu email para confirmar tu cuenta." });
        */
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

            if (user is null)
            {
                return BadRequest("Invalid request.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
                return Unauthorized(new AuthenticationResponseDto { ErrorMessage = "El email no fue confirmado." });

            if (!await _userManager.CheckPasswordAsync(user, authenticationResponse.Password!))
                return Unauthorized(new AuthenticationResponseDto { ErrorMessage = "Autenticación inválida." });

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtHandler.CreateToken(user, roles);
            return Ok(new AuthenticationResponseDto { IsAuthSuccesful = true, Token = token });
        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPassword)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await _userManager.FindByEmailAsync(forgotPassword.Email!);
            if (user is null)
                return BadRequest("Invalid request.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var param = new Dictionary<string, string?>
            {
                { "token", token },
                { "email", forgotPassword.Email! }
            };
            var callback = QueryHelpers.AddQueryString(forgotPassword.ClientUri!, param); 
            
            var message = new Message([user.Email!], "Reset passwrod token", callback, null);

            await _emailSender.SendEmailAsync(message);
            return Ok();
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