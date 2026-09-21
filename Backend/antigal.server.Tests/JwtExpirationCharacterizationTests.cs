using System.IdentityModel.Tokens.Jwt;
using antigal.server.JwtFeatures;
using antigal.server.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Antigal.Server.Tests;

[TestClass]
public class JwtExpirationCharacterizationTests
{
    private static IConfiguration BuildConfigurationWithoutExpiry() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWTSettings:securityKey"] = "01234567890123456789012345678901",
                ["JWTSettings:validIssuer"] = "antigal.test",
                ["JWTSettings:validAudience"] = "antigal.client.test"
            })
            .Build();

    [TestMethod]
    public void HistoricalMissingExpiry_ProducesEffectivelyImmediateTokenExpiration()
    {
        var handler = new JwtHandler(BuildConfigurationWithoutExpiry());
        var user = new User
        {
            Id = "b22-user",
            UserName = "b22@example.test"
        };

        var serialized = handler.CreateToken(user, ["User"]);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(serialized);

        var remaining = token.ValidTo - DateTime.UtcNow;

        Console.WriteLine($"b22_missing_expiry_remaining_seconds={remaining.TotalSeconds:F3}");
        Assert.IsTrue(
            remaining <= TimeSpan.FromSeconds(5),
            $"Expected missing expiry to characterize the historical immediate-expiry behavior, but remaining lifetime was {remaining}."
        );
    }
}
