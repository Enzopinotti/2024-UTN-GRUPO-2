using antigal.server.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace antigal.server.Tests;

[TestClass]
public class ProductCategoryAuthorizationTests
{
    [TestMethod]
    public void Controller_IsAdminAuthorizedByDefault()
    {
        var authorize = typeof(ProductCategoryController)
            .GetCustomAttributes<AuthorizeAttribute>()
            .Single();

        Assert.AreEqual("Admin", authorize.Roles);
        Assert.IsFalse(typeof(ProductCategoryController)
            .IsDefined(typeof(AllowAnonymousAttribute), inherit: true));
    }

    [TestMethod]
    public void MutationEndpoints_RemainAdminProtected()
    {
        AssertAdminMutation(
            nameof(ProductCategoryController.AsignarCategoriaAProductoAsync),
            typeof(HttpPostAttribute),
            "asignar");

        AssertAdminMutation(
            nameof(ProductCategoryController.DesasignarCategoriaDeProductoAsync),
            typeof(HttpDeleteAttribute),
            "desasignar");
    }

    [TestMethod]
    public void ReadEndpoints_RemainExplicitlyAnonymous()
    {
        AssertAnonymousRead(
            nameof(ProductCategoryController.ObtenerCategoriasDeProductoAsync),
            "categorias/{idProducto}");

        AssertAnonymousRead(
            nameof(ProductCategoryController.ObtenerProductosDeCategoriaAsync),
            "productos/{idCategoria}");
    }

    private static void AssertAdminMutation(
        string methodName,
        Type httpAttributeType,
        string expectedTemplate)
    {
        var method = typeof(ProductCategoryController).GetMethod(methodName);
        Assert.IsNotNull(method);

        Assert.IsFalse(
            method.IsDefined(typeof(AllowAnonymousAttribute), inherit: true),
            $"{methodName} must not override the controller Admin policy.");

        var httpAttribute = method.GetCustomAttributes()
            .Single(attribute => attribute.GetType() == httpAttributeType);

        var template = ((HttpMethodAttribute)httpAttribute).Template;
        Assert.AreEqual(expectedTemplate, template);
    }

    private static void AssertAnonymousRead(string methodName, string expectedTemplate)
    {
        var method = typeof(ProductCategoryController).GetMethod(methodName);
        Assert.IsNotNull(method);

        Assert.IsTrue(
            method.IsDefined(typeof(AllowAnonymousAttribute), inherit: true),
            $"{methodName} must remain publicly readable.");

        var httpGet = method.GetCustomAttribute<HttpGetAttribute>();
        Assert.IsNotNull(httpGet);
        Assert.AreEqual(expectedTemplate, httpGet.Template);
    }
}
