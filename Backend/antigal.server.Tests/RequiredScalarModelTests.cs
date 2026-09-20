using System.Text.Json;
using antigal.server.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Antigal.Server.Tests;

[TestClass]
public class RequiredScalarModelTests
{
    [TestMethod]
    public void Contacto_MissingRequiredMember_IsRejectedByJsonContract()
    {
        const string json = """
            {
              "Email": "persona@example.com",
              "Asunto": "Consulta",
              "Mensaje": "Hola"
            }
            """;

        Assert.ThrowsException<JsonException>(() => JsonSerializer.Deserialize<Contacto>(json));
    }

    [TestMethod]
    public void Contacto_PresentEmptyStrings_PreserveExistingNonNullContract()
    {
        const string json = """
            {
              "Name": "",
              "Email": "",
              "Asunto": "",
              "Mensaje": ""
            }
            """;

        var contacto = JsonSerializer.Deserialize<Contacto>(json);

        Assert.IsNotNull(contacto);
        Assert.AreEqual(string.Empty, contacto.Name);
        Assert.AreEqual(string.Empty, contacto.Email);
        Assert.AreEqual(string.Empty, contacto.Asunto);
        Assert.AreEqual(string.Empty, contacto.Mensaje);
    }

    [TestMethod]
    public void Envio_MissingRequiredMember_IsRejectedByJsonContract()
    {
        const string json = """
            {
              "Destinatario": "Persona"
            }
            """;

        Assert.ThrowsException<JsonException>(() => JsonSerializer.Deserialize<Envio>(json));
    }

    [TestMethod]
    public void Envio_PresentEmptyStrings_PreserveExistingNonNullContract()
    {
        const string json = """
            {
              "Destinatario": "",
              "Direccion": ""
            }
            """;

        var envio = JsonSerializer.Deserialize<Envio>(json);

        Assert.IsNotNull(envio);
        Assert.AreEqual(string.Empty, envio.Destinatario);
        Assert.AreEqual(string.Empty, envio.Direccion);
    }
}
