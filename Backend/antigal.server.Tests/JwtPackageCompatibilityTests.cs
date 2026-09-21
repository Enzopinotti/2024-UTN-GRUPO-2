using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using antigal.server.JwtFeatures;
using antigal.server.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Antigal.Server.Tests;

[TestClass]
public class JwtPackageCompatibilityTests
{
    private const string SigningKey = "01234567890123456789012345678901";

    private static IConfiguration BuildConfiguration() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWTSettings:securityKey"] = SigningKey,
                ["JWTSettings:validIssuer"] = "antigal.test",
                ["JWTSettings:validAudience"] = "antigal.client.test",
                ["JWTSettings:expiryInMinutes"] = "60"
            })
            .Build();

    [TestMethod]
    public void JwtHandler_TokenRoundTrip_RemainsValidOnMaintainedIdentityModel()
    {
        var handler = new JwtHandler(BuildConfiguration());
        var user = new User
        {
            Id = "b23-user",
            UserName = "b23@example.test"
        };

        var serialized = handler.CreateToken(user, ["User"]);

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(
            serialized,
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "antigal.test",
                ValidateAudience = true,
                ValidAudience = "antigal.client.test",
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey)),
                ClockSkew = TimeSpan.Zero
            },
            out var validatedToken
        );

        Assert.IsNotNull(principal);
        Assert.IsInstanceOfType(validatedToken, typeof(JwtSecurityToken));

        var jwt = (JwtSecurityToken)validatedToken;
        Assert.AreEqual(SecurityAlgorithms.HmacSha256, jwt.Header.Alg);
        Assert.AreEqual("antigal.test", jwt.Issuer);
        Assert.IsTrue(jwt.Audiences.Contains("antigal.client.test"));
        Assert.IsTrue(jwt.ValidTo > DateTime.UtcNow.AddMinutes(59));
    }
}
