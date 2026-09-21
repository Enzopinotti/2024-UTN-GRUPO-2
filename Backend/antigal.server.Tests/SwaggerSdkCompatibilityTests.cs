using Microsoft.OpenApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Antigal.Server.Tests;

[TestClass]
public class SwaggerSdkCompatibilityTests
{
    [TestMethod]
    public void JwtSecuritySchemeSurfaceRemainsAvailable()
    {
        var scheme = new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: 'Bearer {token}'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        };

        Assert.AreEqual("Authorization", scheme.Name);
        Assert.AreEqual(ParameterLocation.Header, scheme.In);
        Assert.AreEqual(SecuritySchemeType.ApiKey, scheme.Type);
        Assert.AreEqual("Bearer", scheme.Scheme);
    }

    [TestMethod]
    public void SecurityRequirementUsesMicrosoftOpenApiV2ReferenceSurface()
    {
        var document = new OpenApiDocument();
        var reference = new OpenApiSecuritySchemeReference("Bearer", document);
        var requirement = new OpenApiSecurityRequirement
        {
            [reference] = []
        };

        Assert.AreEqual(1, requirement.Count);
        Assert.IsTrue(requirement.ContainsKey(reference));
    }
}
