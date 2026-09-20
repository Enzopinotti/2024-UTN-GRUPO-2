using antigal.server.Controllers;
using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Antigal.Server.Tests;

[TestClass]
public class ProductFilterNullabilityTests
{
    [TestMethod]
    public async Task GetProduct_WithoutFilters_ForwardsNulls()
    {
        var service = new RecordingProductService();
        var controller = new ProductController(service);

        await controller.GetProduct();

        Assert.IsNull(service.LastOrden);
        Assert.IsNull(service.LastPrecio);
    }

    [TestMethod]
    public async Task GetProduct_WithFilters_ForwardsValues()
    {
        var service = new RecordingProductService();
        var controller = new ProductController(service);

        await controller.GetProduct("recientes", "ascendente");

        Assert.AreEqual("recientes", service.LastOrden);
        Assert.AreEqual("ascendente", service.LastPrecio);
    }

    private sealed class RecordingProductService : IProductService
    {
        public string? LastOrden { get; private set; }
        public string? LastPrecio { get; private set; }

        public Task<ResponseDto> GetProducts(string? orden = null, string? precio = null)
        {
            LastOrden = orden;
            LastPrecio = precio;
            return Task.FromResult(new ResponseDto { IsSuccess = true });
        }

        public Task<ResponseDto> GetProductByIdAsync(int id) => NotUsed();
        public Task<ResponseDto> GetProductByTitleAsync(string nombre) => NotUsed();
        public Task<ResponseDto> AddProductAsync(Producto producto) => NotUsed();
        public Task<ResponseDto> DeleteProductAsync(int id) => NotUsed();
        public Task<ResponseDto> PutProductAsync(Producto producto) => NotUsed();
        public Task<ResponseDto> ImportProductsFromExcelAsync(IFormFile file) => NotUsed();
        public Task<ResponseDto> GetProductsByCategoryIdAsync(int categoriaId) => NotUsed();
        public Task<ResponseDto> GetProductsHomeAsync() => NotUsed();

        private static Task<ResponseDto> NotUsed() =>
            Task.FromResult(new ResponseDto { IsSuccess = true });
    }
}
