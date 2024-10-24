using System.Threading.Tasks;
using antigal.server.Models.Dto;
using antigal.server.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace antigal.server.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly HttpClient _httpClient;

        public AuthService(UserManager<User> userManager, HttpClient httpClient)
        {
            _userManager = userManager;
            _httpClient = httpClient;
        }

        public async Task<bool> RegisterUserAsync(RegisterDto registerDto)
        {
            // Crea el objeto de usuario que se enviará a Auth0
            var user = new
            {
                email = registerDto.Email,
                password = registerDto.Password,
                connection = "Username-Password-Authentication" // Asegúrate de usar la conexión correcta
            };

            // Serializa el objeto a JSON
            var json = JsonConvert.SerializeObject(user);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Establece la autenticación en el cliente HTTP (si es necesario)
            // Si la API requiere un token de acceso, puedes agregarlo aquí.

            // Realiza la solicitud POST a la API de Auth0
            var response = await _httpClient.PostAsync("Auth0:Domain", content);

            // Verifica si la respuesta fue exitosa
            return response.IsSuccessStatusCode;
        }

        public async Task<string> LoginUserAsync(LoginDto loginDto)
        {
            var result = await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, false, false);

            if (result.Succeeded)
            {
                return "token";
            }

            return null;
        }
    }
}
