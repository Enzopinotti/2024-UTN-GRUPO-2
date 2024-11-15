using antigal.server.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace antigal.server.JwtFeatures
{
    public class JwtHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _jwtSettings;

        public JwtHandler(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtSettings = _configuration.GetSection("JWTSettings");
        }

        public string CreateToken(User user, IList<string> roles)
        {
            var signingCredentials = GetSigningCredentials();
            var claims = GetClaims(user, roles);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var keyString = _jwtSettings["securityKey"];

            if (string.IsNullOrWhiteSpace(keyString))
            {
                throw new InvalidOperationException("La clave de seguridad JWT no está configurada.");
            }

            var key = Encoding.UTF8.GetBytes(keyString);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private List<Claim> GetClaims(User user, IList<string> roles)
        {
            var claims = new List<Claim>();

            if (!string.IsNullOrWhiteSpace(user.UserName))
            {
                claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            }
            else
            {
                throw new InvalidOperationException("El nombre de usuario no puede ser nulo o vacío.");
            }

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var expiryMinutes = Convert.ToDouble(_jwtSettings["expiryInMinutes"]);
            if (expiryMinutes <= 0)
            {
                throw new InvalidOperationException("El tiempo de expiración del token debe ser mayor a cero.");
            }

            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtSettings["validIssuer"],
                audience: _jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: signingCredentials
            );

            return tokenOptions;
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

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("El token proporcionado es nulo o está vacío.");
                return null;
            }

            // Verificación adicional para la clave de seguridad
            var securityKey = _jwtSettings["securityKey"];
            if (string.IsNullOrWhiteSpace(securityKey))
            {
                Console.WriteLine("La clave de seguridad JWT no está configurada.");
                return null;
            }

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),
                    ValidateLifetime = false // Aquí se desactiva la validación de la expiración
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Token inválido.");
                }

                return principal;
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine($"Error de validación de token: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error desconocido al validar el token: {ex.Message}");
                return null;
            }
        }


    }
}
