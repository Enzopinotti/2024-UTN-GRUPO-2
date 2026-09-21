using MercadoPago.Client.Preference;
using MercadoPago.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Antigal.Server.Tests;

[TestClass]
public class MercadoPagoSdkCompatibilityTests
{
    [TestMethod]
    public void CheckoutProPreferenceSurfaceRemainsAvailable()
    {
        var request = new PreferenceRequest
        {
            Items =
            [
                new PreferenceItemRequest
                {
                    Title = "Producto de prueba",
                    Quantity = 2,
                    CurrencyId = "ARS",
                    UnitPrice = 1250.50m
                }
            ],
            BackUrls = new PreferenceBackUrlsRequest
            {
                Success = "https://example.test/success",
                Failure = "https://example.test/failure",
                Pending = "https://example.test/pending"
            },
            AutoReturn = "approved"
        };

        var client = new PreferenceClient();
        var items = request.Items ?? throw new InvalidOperationException("Preference items were not retained.");
        var backUrls = request.BackUrls ?? throw new InvalidOperationException("Preference back URLs were not retained.");

        Assert.IsNotNull(client);
        Assert.AreEqual(1, items.Count);
        Assert.AreEqual("Producto de prueba", items[0].Title);
        Assert.AreEqual(2, items[0].Quantity);
        Assert.AreEqual("ARS", items[0].CurrencyId);
        Assert.AreEqual(1250.50m, items[0].UnitPrice);
        Assert.AreEqual("https://example.test/success", backUrls.Success);
        Assert.AreEqual("approved", request.AutoReturn);
    }

    [TestMethod]
    public void GlobalAccessTokenConfigurationSurfaceRemainsAvailable()
    {
        var original = MercadoPagoConfig.AccessToken;

        try
        {
            MercadoPagoConfig.AccessToken = "b18-test-token";
            Assert.AreEqual("b18-test-token", MercadoPagoConfig.AccessToken);
        }
        finally
        {
            MercadoPagoConfig.AccessToken = original ?? string.Empty;
        }
    }
}
