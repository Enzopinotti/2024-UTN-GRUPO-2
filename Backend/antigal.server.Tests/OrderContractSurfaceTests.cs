using antigal.server.Repositories;
using antigal.server.Services;

namespace antigal.server.Tests;

[TestClass]
public class OrderContractSurfaceTests
{
    [TestMethod]
    public void OrderServiceContract_ContainsOnlyMaintainedOperations()
    {
        var methods = typeof(IOrderService)
            .GetMethods()
            .Select(method => method.Name)
            .OrderBy(name => name)
            .ToArray();

        CollectionAssert.AreEqual(
            new[]
            {
                nameof(IOrderService.ConfirmOrder),
                nameof(IOrderService.GetAllOrdersAsync),
                nameof(IOrderService.GetOrderByIdAsync),
            },
            methods);
    }

    [TestMethod]
    public void OrderRepositoryContract_ContainsOnlyMaintainedOperations()
    {
        var methods = typeof(IOrderRepository)
            .GetMethods()
            .Select(method => method.Name)
            .OrderBy(name => name)
            .ToArray();

        CollectionAssert.AreEqual(
            new[]
            {
                nameof(IOrderRepository.GetAllOrdersAsync),
                nameof(IOrderRepository.GetOrderByIdAsync),
                nameof(IOrderRepository.GetPendingOrderByUserIdAsync),
                nameof(IOrderRepository.UpdateOrderStatusAsync),
            },
            methods);
    }

    [TestMethod]
    public void OrderService_ImplementationMatchesMaintainedContract()
    {
        var publicMethods = typeof(OrderService)
            .GetMethods()
            .Where(method => method.DeclaringType == typeof(OrderService))
            .Select(method => method.Name)
            .OrderBy(name => name)
            .ToArray();

        CollectionAssert.AreEqual(
            new[]
            {
                nameof(OrderService.ConfirmOrder),
                nameof(OrderService.GetAllOrdersAsync),
                nameof(OrderService.GetOrderByIdAsync),
            },
            publicMethods);
    }

    [TestMethod]
    public void OrderRepository_ImplementationMatchesMaintainedContract()
    {
        var publicMethods = typeof(OrderRepository)
            .GetMethods()
            .Where(method => method.DeclaringType == typeof(OrderRepository))
            .Select(method => method.Name)
            .OrderBy(name => name)
            .ToArray();

        CollectionAssert.AreEqual(
            new[]
            {
                nameof(OrderRepository.GetAllOrdersAsync),
                nameof(OrderRepository.GetOrderByIdAsync),
                nameof(OrderRepository.GetPendingOrderByUserIdAsync),
                nameof(OrderRepository.UpdateOrderStatusAsync),
            },
            publicMethods);
    }
}
