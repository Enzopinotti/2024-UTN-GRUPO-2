using antigal.server.Controllers;
using antigal.server.Models.Dto;
using antigal.server.Models.Dto.CarritoDtos;
using antigal.server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;

namespace antigal.server.Tests;

[TestClass]
public class CartAuthorizationTests
{
    [TestMethod]
    public void Controller_IsAuthenticatedByDefault()
    {
        var authorize = typeof(CartController)
            .GetCustomAttributes<AuthorizeAttribute>()
            .Single();

        Assert.IsNull(authorize.Roles);
        Assert.IsFalse(
            typeof(CartController).IsDefined(typeof(AllowAnonymousAttribute), inherit: true));
    }

    [TestMethod]
    public void Routes_PreserveHistoricalUserIdTemplates()
    {
        var expected = new Dictionary<string, string>
        {
            [nameof(CartController.GetCartByUserIdAsync)] = "{userId}",
            [nameof(CartController.CreateCartAsync)] = "{userId}",
            [nameof(CartController.AddItemToCartAsync)] = "{userId}/items",
            [nameof(CartController.RemoveItemFromCartAsync)] = "{userId}/items/{itemId}",
            [nameof(CartController.ClearCartAsync)] = "{userId}/clear",
            [nameof(CartController.ConfirmCartAsOrderAsync)] = "{userId}/confirmar",
        };

        foreach (var (methodName, template) in expected)
        {
            var method = typeof(CartController).GetMethod(methodName);
            Assert.IsNotNull(method);

            var http = method.GetCustomAttributes()
                .OfType<HttpMethodAttribute>()
                .Single();

            Assert.AreEqual(template, http.Template);
            Assert.IsFalse(method.IsDefined(typeof(AllowAnonymousAttribute), inherit: true));
        }
    }

    [TestMethod]
    public async Task MismatchedUser_IsForbiddenAcrossEveryAction_WithoutServiceCalls()
    {
        var service = new StubCartService();
        var controller = BuildController(service, "owner-user");

        var get = await controller.GetCartByUserIdAsync("other-user");
        var create = await controller.CreateCartAsync("other-user");
        var add = await controller.AddItemToCartAsync(
            "other-user",
            new CarritoItemDto { idProducto = 1, cantidad = 1 });
        var remove = await controller.RemoveItemFromCartAsync("other-user", 7);
        var clear = await controller.ClearCartAsync("other-user");
        var confirm = await controller.ConfirmCartAsOrderAsync("other-user");

        Assert.IsInstanceOfType(get.Result, typeof(ForbidResult));
        Assert.IsInstanceOfType(create.Result, typeof(ForbidResult));
        Assert.IsInstanceOfType(add.Result, typeof(ForbidResult));
        Assert.IsInstanceOfType(remove.Result, typeof(ForbidResult));
        Assert.IsInstanceOfType(clear.Result, typeof(ForbidResult));
        Assert.IsInstanceOfType(confirm, typeof(ForbidResult));
        Assert.AreEqual(0, service.CallCount);
    }

    [TestMethod]
    public async Task MatchingJwtSubject_DelegatesRequestedUserId()
    {
        var service = new StubCartService();
        var controller = BuildController(service, "owner-user");

        var result = await controller.CreateCartAsync("owner-user");

        Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
        Assert.AreEqual(1, service.CallCount);
        Assert.AreEqual("owner-user", service.LastUserId);
    }

    private static CartController BuildController(ICartService service, string userId)
    {
        var identity = new ClaimsIdentity(
            new[] { new Claim(JwtRegisteredClaimNames.Sub, userId) },
            authenticationType: "test",
            nameType: JwtRegisteredClaimNames.Sub,
            roleType: ClaimTypes.Role);

        return new CartController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            }
        };
    }

    private sealed class StubCartService : ICartService
    {
        public int CallCount { get; private set; }
        public string? LastUserId { get; private set; }

        private Task<ResponseDto> Record(string userId)
        {
            CallCount++;
            LastUserId = userId;
            return Task.FromResult(new ResponseDto { IsSuccess = true });
        }

        public Task<ResponseDto> GetCartByUserIdAsync(string userId) => Record(userId);
        public Task<ResponseDto> CreateCartAsync(string userId) => Record(userId);
        public Task<ResponseDto> AddItemToCartAsync(string userId, CarritoItemDto addItemDto) => Record(userId);
        public Task<ResponseDto> RemoveItemFromCartAsync(string userId, int itemId) => Record(userId);
        public Task<ResponseDto> ClearCartAsync(string userId) => Record(userId);
        public Task<ResponseDto> ConfirmCartAsOrderAsync(string userId) => Record(userId);
    }
}
