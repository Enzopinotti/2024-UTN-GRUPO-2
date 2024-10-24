using System.Threading.Tasks;
using antigal.server.Models.Dto;
using antigal.server.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace antigal.server.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<User> userManager, HttpClient httpClient, IConfiguration configuration)
        {
            _userManager = userManager;
            _httpClient = httpClient;
            _configuration = configuration;
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

            // Obtén el dominio de Auth0 desde la configuración
            var domain = _configuration["Auth0:Domain"];
            var url = $"https://{domain}/dbconnections/signup"; // Construye la URL

            // Realiza la solicitud POST a la API de Auth0
            var response = await _httpClient.PostAsync(url, content); // Usa la URL construida

            // Verifica si la respuesta fue exitosa
            if (response.IsSuccessStatusCode)
            {
                // Si el registro en Auth0 fue exitoso, crea un nuevo usuario en tu base de datos
                var newUser = new User
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    // Agrega otros campos que necesites
                };

                var result = await _userManager.CreateAsync(newUser, registerDto.Password);

                return result.Succeeded;
            }

            return false;
        }

        public async Task<UserDto> ValidateUserAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            // Validar el token y extraer los claims
            ClaimsPrincipal principal;
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                // Aquí deberías usar la clave pública si el token está firmado con RS256
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Auth0:ClientSecret"])), // Cambia esto si usas RS256
                ValidateIssuer = true,
                ValidIssuer = $"https://{_configuration["Auth0:Domain"]}/",
                ValidateAudience = true,
                ValidAudience = _configuration["Auth0:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Reduce el tiempo de tolerancia para la expiración
            };

            try
            {
                principal = handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            }
            catch (SecurityTokenExpiredException)
            {
                // El token ha expirado
                return null;
            }
            catch (Exception ex)
            {
                // El token no es válido, puedes registrar el error
                // Logger.LogError(ex, "Token validation failed");
                return null;
            }

            // Extraer información del usuario
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;

            // Crear y devolver un objeto UserDto
            return new UserDto
            {
                Id = userId,
                Email = email
                // Agrega más propiedades según sea necesario
            };
        }

        public async Task<string> LoginUserAsync(LoginDto loginDto)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, $"https://{_configuration["Auth0:Domain"]}/oauth/token")
            {
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    client_id = _configuration["Auth0:ClientId"],
                    client_secret = _configuration["Auth0:ClientSecret"],
                    audience = _configuration["Auth0:Audience"],
                    grant_type = "password",
                    username = loginDto.Email,
                    password = loginDto.Password,
                    scope = "openid profile email"
                }), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

            return tokenResponse.AccessToken; 
        }
    }
}
