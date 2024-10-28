using System.Threading.Tasks;
using antigal.server.Models.Dto;
using antigal.server.Models;
using antigal.server.Services; // Asegúrate de incluir este espacio de nombres
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging; // Para usar ILogger
using antigal.server.Data;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace antigal.server.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ICartService _cartService; // Agregar ICartService
        private readonly ILogger<AuthService> _logger; // Agregar ILogger
        private readonly ServiceToken _serviceToken;
        private readonly AppDbContext _context;
        private readonly IEmailSender _emailSender;

        // Modificar el constructor para inyectar ILogger<AuthService>
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, 
                            ICartService cartService, ILogger<AuthService> logger, 
                            ServiceToken serviceToken, AppDbContext context,
                            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cartService = cartService;
            _logger = logger;
            _serviceToken = serviceToken;
            _context = context;
            _emailSender = emailSender;
        }

        public async Task<bool> RegisterUserAsync(RegisterDto registerDto)
        {
            var user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
            };


            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                // Generar el token JWT para la confirmación del correo electrónico
                var token = _serviceToken.GenerateEmailConfirmationToken(user); // Método que deberás implementar
                var confirmationLink = $"https://localhost:7255/api/auth/confirmemail?userId={user.Id}&token={token}";

                // Enviar el correo electrónico de confirmación
                await _emailSender.SendEmailAsync(
                    registerDto.Email,
                    "Confirma tu cuenta",
                    $"Por favor confirma tu cuenta haciendo clic aquí: <a href='{confirmationLink}'>link</a>");

                // Crear un carrito vacío para el nuevo usuario
                var cartResponse = _cartService.CreateCart(user.Id);
                if (!cartResponse.IsSuccess)
                {
                    // Registrar un mensaje de error si la creación del carrito falla
                    _logger.LogError($"No se pudo crear el carrito para el usuario con ID: {user.Id}. Error: {cartResponse.Message}");
                }

                return true; // Registro exitoso
            }

            return false; // Registro fallido
        }

        public async Task<(string AccessToken, string RefreshToken)> LoginUserAsync(LoginDto loginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(loginDto.UserName);
                var accessToken = _serviceToken.GenerateToken(user); // Genera el token de acceso
                var refreshToken = _serviceToken.GenerateRefreshToken(); // Genera el refresh token

                // Almacenar el refresh token en la base de datos
                var tokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = user.Id,
                    Expiration = DateTime.UtcNow.AddDays(1) // Establecer una expiración (por ejemplo, 7 días)
                };
                _context.RefreshTokens.Add(tokenEntity);
                await _context.SaveChangesAsync();

                return (accessToken, refreshToken);
            }

            return (null, null); // Retornar null si el inicio de sesión falla
        }

        public async Task<string> RefreshTokenAsync(string refreshToken)
        {
            // Busca el token de refresco en la base de datos
            var tokenEntity = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);

            // Verifica si el token no existe o ha expirado
            if (tokenEntity == null || tokenEntity.Expiration < DateTime.UtcNow)
            {
                return null; // El refresh token no es válido o ha expirado
            }

            // Encuentra al usuario asociado al token
            var user = await _userManager.FindByIdAsync(tokenEntity.UserId);
            if (user == null)
            {
                return null; // El usuario no existe
            }

            // Genera un nuevo access token y un nuevo refresh token
            var newAccessToken = _serviceToken.GenerateToken(user);
            var newRefreshToken = _serviceToken.GenerateRefreshToken();

            // Actualiza el refresh token en la base de datos
            tokenEntity.Token = newRefreshToken;
            tokenEntity.Expiration = DateTime.UtcNow.AddDays(7); // Actualizar la expiración
            await _context.SaveChangesAsync();

            // Retorna el nuevo token de acceso
            return newAccessToken;
        }

        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false; // Usuario no encontrado
            }

            // Confirmar el correo electrónico
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded; // Retorna true si la confirmación fue exitosa
        }
    }
}
