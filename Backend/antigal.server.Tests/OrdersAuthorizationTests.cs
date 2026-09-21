using antigal.server.Controllers;
using antigal.server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace antigal.server.Tests;

[TestClass]
public class OrdersAuthorizationTests
{
    [TestMethod]
    public void Controller_IsAdminAuthorizedByDefault()
    {
        var authorize = typeof(OrdersController)
            .GetCustomAttributes<AuthorizeAttribute>()
            .Single();

        Assert.AreEqual("Admin", authorize.Roles);
        Assert.IsFalse(
            typeof(OrdersController).IsDefined(typeof(AllowAnonymousAttribute), inherit: true));
    }

    [TestMethod]
    public void MaintainedRoutes_InheritAdminPolicy()
    {
        var expected = new Dictionary<string, (Type AttributeType, string Template)>
        {
            [nameof(OrdersController.GetAllOrders)] = (typeof(HttpGetAttribute), "all"),
            [nameof(OrdersController.ConfirmOrder)] = (typeof(HttpPostAttribute), "confirm/{orderId}"),
        };

        foreach (var (methodName, route) in expected)
        {
            var method = typeof(OrdersController).GetMethod(methodName);
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
    public void Constructor_DependsOnlyOnOrderService()
    {
        var constructors = typeof(OrdersController).GetConstructors();

        Assert.AreEqual(1, constructors.Length);
        CollectionAssert.AreEqual(
            new[] { typeof(IOrderService) },
            constructors[0].GetParameters().Select(parameter => parameter.ParameterType).ToArray());
    }
}
