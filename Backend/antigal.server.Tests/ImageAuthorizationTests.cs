using antigal.server.Controllers;
using antigal.server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace antigal.server.Tests;

[TestClass]
public class ImageAuthorizationTests
{
    [TestMethod]
    public void Controller_IsAdminAuthorizedByDefault()
    {
        var authorize = typeof(ImageController)
            .GetCustomAttributes<AuthorizeAttribute>()
            .Single();

        Assert.AreEqual("Admin", authorize.Roles);
        Assert.IsFalse(
            typeof(ImageController).IsDefined(typeof(AllowAnonymousAttribute), inherit: true));
    }

    [TestMethod]
    public void MaintainedRoutes_InheritAdminPolicy()
    {
        var expected = new Dictionary<string, (Type AttributeType, string Template)>
        {
            [nameof(ImageController.Upload)] = (typeof(HttpPostAttribute), "upload"),
            [nameof(ImageController.DeleteImage)] = (typeof(HttpDeleteAttribute), "{imageId}"),
            [nameof(ImageController.DeleteImageByUrl)] = (typeof(HttpDeleteAttribute), "eliminar-por-url"),
        };

        foreach (var (methodName, route) in expected)
        {
            var method = typeof(ImageController).GetMethod(methodName);
            Assert.IsNotNull(method);

            Assert.IsFalse(
                method.IsDefined(typeof(AllowAnonymousAttribute), inherit: true),
                $"{methodName} must not override the controller Admin policy.");

            var http = method.GetCustomAttributes()
                .Single(attribute => attribute.GetType() == route.AttributeType);

            Assert.AreEqual(route.Template, ((HttpMethodAttribute)http).Template);
        }
    }

    [TestMethod]
    public void Constructor_DependsOnlyOnImageService()
    {
        var constructors = typeof(ImageController).GetConstructors();

        Assert.AreEqual(1, constructors.Length);
        CollectionAssert.AreEqual(
            new[] { typeof(IImageService) },
            constructors[0].GetParameters().Select(parameter => parameter.ParameterType).ToArray());
    }
}
