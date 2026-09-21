using antigal.server.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace antigal.server.Tests;

[TestClass]
public class PaymentAuthorizationTests
{
    [TestMethod]
    public void Controller_IsAuthenticatedByDefault()
    {
        var authorize = typeof(PaymentController)
            .GetCustomAttributes<AuthorizeAttribute>()
            .Single();

        Assert.IsNull(authorize.Roles);
        Assert.IsFalse(typeof(PaymentController)
            .IsDefined(typeof(AllowAnonymousAttribute), inherit: true));
    }

    [TestMethod]
    public void CreatePayment_RemainsAuthenticated()
    {
        var method = typeof(PaymentController).GetMethod(
            nameof(PaymentController.CreatePayment));

        Assert.IsNotNull(method);
        Assert.IsFalse(
            method.IsDefined(typeof(AllowAnonymousAttribute), inherit: true));

        var httpPost = method.GetCustomAttribute<HttpPostAttribute>();
        Assert.IsNotNull(httpPost);
        Assert.AreEqual("create-payment", httpPost.Template);
    }

    [TestMethod]
    public void Controller_HasNoAnonymousActions()
    {
        var anonymousActions = typeof(PaymentController)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(method => method.IsDefined(typeof(AllowAnonymousAttribute), inherit: true))
            .Select(method => method.Name)
            .ToArray();

        CollectionAssert.AreEqual(Array.Empty<string>(), anonymousActions);
    }
}
