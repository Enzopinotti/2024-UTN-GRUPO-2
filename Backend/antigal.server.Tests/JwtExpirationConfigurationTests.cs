using System.IdentityModel.Tokens.Jwt;
using antigal.server.JwtFeatures;
using antigal.server.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Antigal.Server.Tests;

[TestClass]
public class JwtExpirationConfigurationTests
{
    private static IConfiguration BuildConfiguration(string? expiryInMinutes) =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWTSettings:securityKey"] = "01234567890123456789012345678901",
                ["JWTSettings:validIssuer"] = "antigal.test",
                ["JWTSettings:validAudience"] = "antigal.client.test",
                ["JWTSettings:expiryInMinutes"] = expiryInMinutes
            })
            .Build();

    private static User CreateUser() => new()
    {
        Id = "b22-user",
        UserName = "b22@example.test"
    };

    [TestMethod]
    public void ConfiguredPositiveExpiry_ProducesTokenWithExpectedFutureLifetime()
    {
        var handler = new JwtHandler(BuildConfiguration("60"));

        var serialized = handler.CreateToken(CreateUser(), ["User"]);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(serialized);
        var remaining = token.ValidTo - DateTime.UtcNow;

        Assert.IsTrue(
            remaining > TimeSpan.FromMinutes(59),
            $"Expected approximately 60 minutes of JWT lifetime, but remaining lifetime was {remaining}."
        );
        Assert.IsTrue(
            remaining <= TimeSpan.FromMinutes(61),
            $"JWT lifetime exceeded the configured 60-minute boundary: {remaining}."
        );
    }

    [TestMethod]
    public void MissingExpiry_IsRejectedInsteadOfCreatingImmediatelyExpiredToken()
    {
        var handler = new JwtHandler(BuildConfiguration(null));

        var exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => handler.CreateToken(CreateUser(), ["User"])
        );

        StringAssert.Contains(exception.Message, "expiryInMinutes");
    }

    [TestMethod]
    public void NonPositiveExpiry_IsRejected()
    {
        var handler = new JwtHandler(BuildConfiguration("0"));

        var exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => handler.CreateToken(CreateUser(), ["User"])
        );

        StringAssert.Contains(exception.Message, "entero positivo");
    }
}
