using EmailService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Antigal.Server.Tests;

[TestClass]
public class EmailServiceRegistrationTests
{
    private static EmailConfiguration CreateConfiguration() => new()
    {
        From = "sender@example.test",
        SmtpServer = "smtp.example.test",
        Port = 587,
        UserName = "test-user",
        Password = "test-password"
    };

    [TestMethod]
    public void HistoricalDuplicateRegistration_DirectResolutionUsesLastTransientRegistration()
    {
        var services = new ServiceCollection();
        services.AddSingleton(CreateConfiguration());
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddTransient<IEmailSender, EmailSender>();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var first = scope.ServiceProvider.GetRequiredService<IEmailSender>();
        var second = scope.ServiceProvider.GetRequiredService<IEmailSender>();
        var all = scope.ServiceProvider.GetServices<IEmailSender>().ToArray();

        Assert.AreEqual(2, all.Length);
        Assert.AreNotSame(first, second);
        Assert.IsInstanceOfType(first, typeof(EmailSender));
        Assert.IsInstanceOfType(second, typeof(EmailSender));
    }

    [TestMethod]
    public void CanonicalTransientRegistration_PreservesHistoricalDirectResolutionLifetime()
    {
        var services = new ServiceCollection();
        services.AddSingleton(CreateConfiguration());
        services.AddTransient<IEmailSender, EmailSender>();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var first = scope.ServiceProvider.GetRequiredService<IEmailSender>();
        var second = scope.ServiceProvider.GetRequiredService<IEmailSender>();
        var all = scope.ServiceProvider.GetServices<IEmailSender>().ToArray();

        Assert.AreEqual(1, all.Length);
        Assert.AreNotSame(first, second);
        Assert.IsInstanceOfType(first, typeof(EmailSender));
        Assert.IsInstanceOfType(second, typeof(EmailSender));
    }
}
