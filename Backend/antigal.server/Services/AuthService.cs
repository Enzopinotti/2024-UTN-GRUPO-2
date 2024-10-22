using System.Threading.Tasks;
using antigal.server.Models.Dto;
using antigal.server.Models;
using antigal.server.Services; // Asegúrate de incluir este espacio de nombres
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging; // Para usar ILogger

namespace antigal.server.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ICartService _cartService; // Agregar ICartService
        private readonly ILogger<AuthService> _logger; // Agregar ILogger

        // Modificar el constructor para inyectar ILogger<AuthService>
        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, ICartService cartService, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cartService = cartService; // Inicializar ICartService
            _logger = logger; // Inicializar ILogger
        }

        public async Task<bool> RegisterUserAsync(RegisterDto registerDto)
        {
            var user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                FullName = registerDto.FullName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
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

        public async Task<string> LoginUserAsync(LoginDto loginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, false, false);

            if (result.Succeeded)
            {
                return "token"; // Generar un token de JWT aquí si lo necesitas
            }

            return null; // Retornar null si el inicio de sesión falla
        }
    }
}
