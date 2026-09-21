using antigal.server.Data;

namespace antigal.server.Tests;

[TestClass]
public class AdminBootstrapPolicyTests
{
    [TestMethod]
    public void Options_DefaultToDisabledWithoutCredentials()
    {
        var options = new AdminBootstrapOptions();

        Assert.IsFalse(options.Enabled);
        Assert.IsNull(options.UserName);
        Assert.IsNull(options.Email);
        Assert.IsNull(options.Password);
    }

    [TestMethod]
    public void Resolve_Disabled_ReturnsNullOutsideDevelopment()
    {
        var options = new AdminBootstrapOptions
        {
            Enabled = false
        };

        var result = AdminBootstrapPolicy.Resolve(options, isDevelopment: false);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void Resolve_EnabledOutsideDevelopment_IsRejected()
    {
        var options = ValidOptions();

        Assert.ThrowsExactly<InvalidOperationException>(() =>
            AdminBootstrapPolicy.Resolve(options, isDevelopment: false));
    }

    [TestMethod]
    public void Resolve_EnabledDevelopmentWithoutCompleteConfiguration_IsRejected()
    {
        var options = ValidOptions();
        options.Password = "";

        Assert.ThrowsExactly<InvalidOperationException>(() =>
            AdminBootstrapPolicy.Resolve(options, isDevelopment: true));
    }

    [TestMethod]
    public void Resolve_EnabledDevelopment_ReturnsConfiguredCredentials()
    {
        var options = ValidOptions();

        var result = AdminBootstrapPolicy.Resolve(options, isDevelopment: true);

        Assert.IsNotNull(result);
        Assert.AreEqual(options.UserName, result.UserName);
        Assert.AreEqual(options.Email, result.Email);
        Assert.AreEqual(options.Password, result.Password);
    }

    private static AdminBootstrapOptions ValidOptions() =>
        new()
        {
            Enabled = true,
            UserName = "bootstrap-admin",
            Email = "bootstrap@example.test",
            Password = "Example-only-strong-password-123!"
        };
}
