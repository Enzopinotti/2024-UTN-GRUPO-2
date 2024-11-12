using antigal.server.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace antigal.server.Services
{
    public class ServiceToken
    {
        private readonly IConfiguration _configuration;

        public ServiceToken(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GenerateToken(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user), "El usuario no puede ser nulo.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? throw new ArgumentNullException(nameof(user.UserName), "El nombre de usuario no puede ser nulo."))
                // Puedes agregar más claims aquí si lo necesitas
            };

            var keyString = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key", "La clave JWT no puede ser nula.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer", "El emisor JWT no puede ser nulo."),
                audience: _configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience", "La audiencia JWT no puede ser nula."),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public string GenerateEmailConfirmationToken(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user), "El usuario no puede ser nulo.");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("email_confirm", "true") // Puedes agregar un claim para indicar que es para la confirmación de email
            };

            var keyString = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key", "La clave JWT no puede ser nula.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer", "El emisor JWT no puede ser nulo."),
                audience: _configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience", "La audiencia JWT no puede ser nula."),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            if (string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token), "El token no puede ser nulo o vacío.");

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"] ?? throw new ArgumentNullException("Jwt:Issuer", "El emisor JWT no puede ser nulo."),
                ValidAudience = _configuration["Jwt:Audience"] ?? throw new ArgumentNullException("Jwt:Audience", "La audiencia JWT no puede ser nula."),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key", "La clave JWT no puede ser nula.")))
            };

            SecurityToken securityToken;
            return tokenHandler.ValidateToken(token, validationParameters, out securityToken);
        }
    }
}