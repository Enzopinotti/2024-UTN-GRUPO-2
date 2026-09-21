using MercadoPago.Client.Preference;
using MercadoPago.Config;
using Xunit;

namespace Antigal.Server.Tests;

public class MercadoPagoSdkCompatibilityTests
{
    [Fact]
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

        Assert.NotNull(client);
        Assert.Single(request.Items);
        Assert.Equal("Producto de prueba", request.Items[0].Title);
        Assert.Equal(2, request.Items[0].Quantity);
        Assert.Equal("ARS", request.Items[0].CurrencyId);
        Assert.Equal(1250.50m, request.Items[0].UnitPrice);
        Assert.Equal("https://example.test/success", request.BackUrls.Success);
        Assert.Equal("approved", request.AutoReturn);
    }

    [Fact]
    public void GlobalAccessTokenConfigurationSurfaceRemainsAvailable()
    {
        var original = MercadoPagoConfig.AccessToken;

        try
        {
            MercadoPagoConfig.AccessToken = "b18-test-token";
            Assert.Equal("b18-test-token", MercadoPagoConfig.AccessToken);
        }
        finally
        {
            MercadoPagoConfig.AccessToken = original;
        }
    }
}
