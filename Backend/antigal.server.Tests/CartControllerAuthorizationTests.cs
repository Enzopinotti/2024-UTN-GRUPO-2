using antigal.server.Controllers;
using antigal.server.Models.Dto;
using antigal.server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;

namespace antigal.server.Tests;

[TestClass]
public class CartControllerAuthorizationTests
{
    [TestMethod]
    public void Controller_RequiresAuthentication()
    {
        var attribute = typeof(CartController)
            .GetCustomAttribute<AuthorizeAttribute>();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public async Task ForeignRouteUserId_BlocksEveryCartOperationBeforeService()
    {
        var service = new StubCartService();
        var controller = Controller(service, Claim(JwtRegisteredClaimNames.Sub, "user-owner"));

        var get = await controller.GetCartByUserIdAsync("user-other");
        var create = await controller.CreateCartAsync("user-other");
        var add = await controller.AddItemToCartAsync(
            "user-other",
            new CarritoItemDto { idProducto = 10, cantidad = 1, precioUnitario = 5m });
        var remove = await controller.RemoveItemFromCartAsync("user-other", 20);
        var clear = await controller.ClearCartAsync("user-other");
        var confirm = await controller.ConfirmCartAsOrderAsync("user-other");

        Assert.IsInstanceOfType<ForbidResult>(get.Result);
        Assert.IsInstanceOfType<ForbidResult>(create.Result);
        Assert.IsInstanceOfType<ForbidResult>(add.Result);
        Assert.IsInstanceOfType<ForbidResult>(remove.Result);
        Assert.IsInstanceOfType<ForbidResult>(clear.Result);
        Assert.IsInstanceOfType<ForbidResult>(confirm);
        Assert.AreEqual(0, service.TotalCalls);
    }

    [TestMethod]
    public async Task MatchingSubjectClaim_AllowsEveryCartOperation()
    {
        var service = new StubCartService();
        var controller = Controller(service, Claim(JwtRegisteredClaimNames.Sub, "user-owner"));

        await controller.GetCartByUserIdAsync("user-owner");
        await controller.CreateCartAsync("user-owner");
        await controller.AddItemToCartAsync(
            "user-owner",
            new CarritoItemDto { idProducto = 10, cantidad = 1, precioUnitario = 5m });
        await controller.RemoveItemFromCartAsync("user-owner", 20);
        await controller.ClearCartAsync("user-owner");
        await controller.ConfirmCartAsOrderAsync("user-owner");

        Assert.AreEqual(6, service.TotalCalls);
        Assert.AreEqual("user-owner", service.LastUserId);
    }

    [TestMethod]
    public async Task NameIdentifierClaim_IsAcceptedAsJwtMappingFallback()
    {
        var service = new StubCartService();
        var controller = Controller(service, Claim(ClaimTypes.NameIdentifier, "mapped-user"));

        await controller.CreateCartAsync("mapped-user");

        Assert.AreEqual(1, service.CreateCalls);
        Assert.AreEqual("mapped-user", service.LastUserId);
    }

    [TestMethod]
    public async Task MissingUserClaim_ReturnsUnauthorizedBeforeService()
    {
        var service = new StubCartService();
        var controller = Controller(service, new ClaimsPrincipal(new ClaimsIdentity(authenticationType: "test")));

        var result = await controller.GetCartByUserIdAsync("user-owner");

        Assert.IsInstanceOfType<UnauthorizedObjectResult>(result.Result);
        Assert.AreEqual(0, service.TotalCalls);
    }

    private static Claim Claim(string type, string value) => new(type, value);

    private static CartController Controller(StubCartService service, Claim claim)
    {
        return Controller(
            service,
            new ClaimsPrincipal(new ClaimsIdentity([claim], authenticationType: "test")));
    }

    private static CartController Controller(StubCartService service, ClaimsPrincipal user)
    {
        return new CartController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            }
        };
    }

    private sealed class StubCartService : ICartService
    {
        public int GetCalls { get; private set; }
        public int CreateCalls { get; private set; }
        public int AddCalls { get; private set; }
        public int RemoveCalls { get; private set; }
        public int ClearCalls { get; private set; }
        public int ConfirmCalls { get; private set; }
        public string? LastUserId { get; private set; }

        public int TotalCalls =>
            GetCalls + CreateCalls + AddCalls + RemoveCalls + ClearCalls + ConfirmCalls;

        public Task<ResponseDto> GetCartByUserIdAsync(string userId)
        {
            GetCalls++;
            LastUserId = userId;
            return Task.FromResult(Success(new CarritoDto()));
        }

        public Task<ResponseDto> CreateCartAsync(string userId)
        {
            CreateCalls++;
            LastUserId = userId;
            return Task.FromResult(Success());
        }

        public Task<ResponseDto> AddItemToCartAsync(string userId, CarritoItemDto addItemDto)
        {
            AddCalls++;
            LastUserId = userId;
            return Task.FromResult(Success());
        }

        public Task<ResponseDto> RemoveItemFromCartAsync(string userId, int itemId)
        {
            RemoveCalls++;
            LastUserId = userId;
            return Task.FromResult(Success());
        }

        public Task<ResponseDto> ClearCartAsync(string userId)
        {
            ClearCalls++;
            LastUserId = userId;
            return Task.FromResult(Success());
        }

        public Task<ResponseDto> ConfirmCartAsOrderAsync(string userId)
        {
            ConfirmCalls++;
            LastUserId = userId;
            return Task.FromResult(Success());
        }

        private static ResponseDto Success(object? data = null) =>
            new()
            {
                IsSuccess = true,
                Data = data
            };
    }
}
