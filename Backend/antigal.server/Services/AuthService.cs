/*using System.Threading.Tasks;
using antigal.server.Models.Dto;
using antigal.server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using antigal.server.Data;
using Microsoft.EntityFrameworkCore;
using EmailService;

namespace antigal.server.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ICartService _cartService;
        private readonly ILogger<AuthService> _logger;
        private readonly ServiceToken _serviceToken;
        private readonly AppDbContext _context;
        private readonly IEmailSender _emailSender;

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
            try
            {
                var user = new User
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    var token = _serviceToken.GenerateEmailConfirmationToken(user);
                    var confirmationLink = $"https://localhost:7255/api/auth/confirmemail?userId={user.Id}&token={token}";

                    try
                    {
                        await _emailSender.SendEmailAsync(
                            registerDto.Email,
                            "Confirma tu cuenta",
                            $"Por favor confirma tu cuenta haciendo clic aquí: <a href='{confirmationLink}'>link</a>");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error al enviar correo de confirmación al usuario con ID: {user.Id}. Excepción: {ex.Message}");
                    }

                    var cartResponse = _cartService.CreateCart(user.Id);
                    if (!cartResponse.IsSuccess)
                    {
                        _logger.LogError($"No se pudo crear el carrito para el usuario con ID: {user.Id}. Error: {cartResponse.Message}");
                    }

                    return true;
                }

                _logger.LogWarning("Error al registrar usuario: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                return false;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error al interactuar con la base de datos en el registro de usuario. Excepción: {ex.Message}");
                throw new InvalidOperationException("Error al registrar el usuario en la base de datos.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Excepción no controlada al registrar usuario. Excepción: {ex.Message}");
                throw;
            }
        }

        public async Task<(string AccessToken, string RefreshToken)> LoginUserAsync(LoginDto loginDto)
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, false, false);

                if (!result.Succeeded)
                {
                    throw new UnauthorizedAccessException("El inicio de sesión ha fallado debido a credenciales inválidas.");
                }

                var user = await _userManager.FindByNameAsync(loginDto.UserName);
                if (user == null)
                {
                    throw new KeyNotFoundException("Usuario no encontrado en la base de datos.");
                }

                var accessToken = _serviceToken.GenerateToken(user);
                var refreshToken = _serviceToken.GenerateRefreshToken();

                var tokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = user.Id,
                    Expiration = DateTime.UtcNow.AddDays(1)
                };

                _context.RefreshTokens.Add(tokenEntity);
                await _context.SaveChangesAsync();

                return (accessToken, refreshToken);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error al interactuar con la base de datos en el inicio de sesión. Excepción: {ex.Message}");
                throw new InvalidOperationException("Error al guardar el token de inicio de sesión en la base de datos.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Excepción no controlada durante el inicio de sesión para el usuario: {loginDto.UserName}. Excepción: {ex.Message}");
                throw;
            }
        }

        public async Task<string> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                // Buscar el token en la base de datos
                var tokenEntity = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);

                // Verificar si el token es nulo o ha expirado
                if (tokenEntity == null)
                {
                    _logger.LogWarning("El token de refresco no es válido o no existe.");
                    return string.Empty; // Retorna una cadena vacía
                }

                if (tokenEntity.Expiration < DateTime.UtcNow)
                {
                    _logger.LogWarning("El token de refresco ha expirado.");
                    return string.Empty; // Retorna una cadena vacía
                }

                // Verificar que tokenEntity.UserId no sea nulo
                var userId = tokenEntity.UserId; // Asegúrate de que UserId no sea nulo
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _logger.LogWarning("UserId asociado al token de refresco es nulo o vacío.");
                    return string.Empty; // Retorna una cadena vacía
                }

                // Buscar el usuario asociado al UserId
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"Usuario asociado al token de refresco no encontrado para UserId: {userId}.");
                    return string.Empty; // Retorna una cadena vacía
                }

                // Generar nuevos tokens
                var newAccessToken = _serviceToken.GenerateToken(user);
                var newRefreshToken = _serviceToken.GenerateRefreshToken();

                // Actualizar el token en la base de datos
                tokenEntity.Token = newRefreshToken;
                tokenEntity.Expiration = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                return newAccessToken;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error al actualizar el token en la base de datos. Excepción: {ex.Message}");
                throw new InvalidOperationException("Error al actualizar el token de acceso en la base de datos.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inesperado al actualizar el token de acceso con el token de refresco. Excepción: {ex.Message}");
                throw;
            }
        }




        public async Task<User> GetUserByIdAsync(string userId)
        {
            // Verificar si userId es nulo o vacío
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("userId es nulo o vacío.");
                throw new ArgumentNullException(nameof(userId), "El ID del usuario no puede ser nulo o vacío.");
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"Usuario con ID {userId} no encontrado.");
                    throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado.");
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Excepción al obtener el usuario con ID: {userId}. Excepción: {ex.Message}");
                throw;
            }
        }



        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            // Verificar si userId es nulo o vacío
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("userId es nulo o vacío.");
                return false; // O lanzar una excepción, según tu lógica
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"Usuario con ID {userId} no encontrado para confirmación de correo.");
                    return false;
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);
                return result.Succeeded;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError($"Token de confirmación inválido para el usuario con ID: {userId}. Excepción: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al confirmar el correo electrónico para el usuario con ID: {userId}. Excepción: {ex.Message}");
                throw;
            }
        }

    }
}
*/