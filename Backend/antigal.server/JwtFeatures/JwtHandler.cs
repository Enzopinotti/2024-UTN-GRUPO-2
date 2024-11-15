using antigal.server.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
                // Agrega el claim 'sub' con el user.Id
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
                claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            }
            else
            {
                throw new InvalidOperationException("El nombre de usuario no puede ser nulo o vacío.");
            }
            foreach (var role in roles) {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtSettings["validIssuer"],
                audience: _jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings["expiryInMinutes"])),
                signingCredentials: signingCredentials
            );

            return tokenOptions;
        }

    }
}
