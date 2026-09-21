using antigal.server.Controllers;
using EmailService;

namespace antigal.server.Tests;

[TestClass]
public class DeadEmailTestSurfaceTests
{
    [TestMethod]
    public void ProductionAssembly_DoesNotExposeLegacyEmailTestTypes()
    {
        var assembly = typeof(Program).Assembly;
        var names = assembly
            .GetTypes()
            .Select(type => type.FullName)
            .Where(name => name is not null)
            .Cast<string>()
            .ToArray();

        CollectionAssert.DoesNotContain(
            names,
            "antigal.server.Controllers.EmailController");
        CollectionAssert.DoesNotContain(
            names,
            "antigal.server.Models.Dto.TestEmailDto");
    }

    [TestMethod]
    public void AccountController_StillDependsOnProductionEmailSender()
    {
        var constructor = typeof(AccountController).GetConstructors().Single();
        var parameterTypes = constructor
            .GetParameters()
            .Select(parameter => parameter.ParameterType)
            .ToArray();

        CollectionAssert.Contains(parameterTypes, typeof(IEmailSender));
    }
}
