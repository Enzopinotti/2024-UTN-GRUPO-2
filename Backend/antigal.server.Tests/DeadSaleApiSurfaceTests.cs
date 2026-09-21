using antigal.server.Controllers;
using antigal.server.Repositories;
using antigal.server.Services;

namespace antigal.server.Tests;

[TestClass]
public class DeadSaleApiSurfaceTests
{
    [TestMethod]
    public void ProductionAssembly_DoesNotExposeSaleController()
    {
        var assembly = typeof(OrdersController).Assembly;

        Assert.IsNull(assembly.GetType("SaleController"));
    }

    [TestMethod]
    public void SaleServiceTypes_AreAbsent()
    {
        var assembly = typeof(OrderService).Assembly;

        Assert.IsNull(assembly.GetType("antigal.server.Services.SaleService"));
        Assert.IsNull(assembly.GetType("antigal.server.Services.ISaleService"));
    }

    [TestMethod]
    public void OrdersController_Constructor_DependsOnlyOnOrderService()
    {
        var constructors = typeof(OrdersController).GetConstructors();

        Assert.AreEqual(1, constructors.Length);
        CollectionAssert.AreEqual(
            new[] { typeof(IOrderService) },
            constructors[0].GetParameters().Select(parameter => parameter.ParameterType).ToArray());
    }

    [TestMethod]
    public void SaleRepositoryContract_IsCreateOnly()
    {
        var methods = typeof(ISaleRepository).GetMethods();

        Assert.AreEqual(1, methods.Length);
        Assert.AreEqual(nameof(ISaleRepository.CreateSaleAsync), methods[0].Name);
    }

    [TestMethod]
    public void LegacySaleDtos_AreAbsent()
    {
        var assembly = typeof(OrderService).Assembly;

        Assert.IsNull(assembly.GetType("antigal.server.Models.Dto.VentaDtos.SaleRequestDto"));
        Assert.IsNull(assembly.GetType("antigal.server.Models.Dto.VentaDtos.SaleResponseDto"));
        Assert.IsNull(assembly.GetType("antigal.server.Models.Dto.SaleDto"));
    }
}
