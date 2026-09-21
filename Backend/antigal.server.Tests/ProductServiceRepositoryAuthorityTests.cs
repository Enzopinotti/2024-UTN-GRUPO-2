using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Repositories;
using antigal.server.Services;
using Microsoft.EntityFrameworkCore.Storage;

namespace antigal.server.Tests;

[TestClass]
public class ProductServiceRepositoryAuthorityTests
{
    [TestMethod]
    public async Task GetProducts_UsesUnitOfWorkProducts_AndPreservesResponseContract()
    {
        var first = Product(1, "Alpha");
        var second = Product(2, "Beta");
        var repository = new StubProductRepository
        {
            GetProductsHandler = (orden, precio) =>
            {
                Assert.AreEqual("recientes", orden);
                Assert.AreEqual("descendente", precio);
                return Task.FromResult<IEnumerable<Producto>>([first, second]);
            }
        };
        var service = new ProductService(new StubUnitOfWork(repository));

        var result = await service.GetProducts("recientes", "descendente");
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Productos obtenidos correctamente.", result.Message);
        var products = result.Data as List<Producto>;
        Assert.IsNotNull(products);
        Assert.AreEqual(2, products.Count);
        Assert.AreSame(first, products[0]);
        Assert.AreSame(second, products[1]);
        Assert.AreEqual(1, repository.GetProductsCalls);
    }

    [TestMethod]
    public async Task GetProductsHome_UsesUnitOfWorkProducts_ForFeaturedProducts()
    {
        var featured = Product(10, "Featured");
        var repository = new StubProductRepository
        {
            GetFeaturedProductsHandler = () => Task.FromResult(new List<Producto> { featured })
        };
        var service = new ProductService(new StubUnitOfWork(repository));

        var result = await service.GetProductsHomeAsync();

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Productos destacados obtenidos correctamente.", result.Message);
        var products = result.Data as List<Producto>;
        Assert.IsNotNull(products);
        Assert.AreEqual(1, products.Count);
        Assert.AreSame(featured, products[0]);
        Assert.AreEqual(1, repository.GetFeaturedProductsCalls);
    }

    [TestMethod]
    public async Task GetProductsHome_PreservesEmptyFeaturedProductsContract()
    {
        var repository = new StubProductRepository
        {
            GetFeaturedProductsHandler = () => Task.FromResult(new List<Producto>())
        };
        var service = new ProductService(new StubUnitOfWork(repository));

        var result = await service.GetProductsHomeAsync();

        Assert.IsFalse(result.IsSuccess);
        Assert.IsNull(result.Data);
        Assert.AreEqual("No se encontraron productos destacados.", result.Message);
        Assert.AreEqual(1, repository.GetFeaturedProductsCalls);
    }

    [TestMethod]
    public async Task GetProducts_ReturnsIndependentResponseObjectsAcrossCalls()
    {
        var repository = new StubProductRepository
        {
            GetProductsHandler = (_, _) =>
                Task.FromResult<IEnumerable<Producto>>([Product(20, "Independent")])
        };
        var service = new ProductService(new StubUnitOfWork(repository));

        var first = await service.GetProducts();
        first.Message = "mutated by caller";
        var second = await service.GetProducts();

        Assert.AreNotSame(first, second);
        Assert.AreEqual("Productos obtenidos correctamente.", second.Message);
        Assert.IsTrue(second.IsSuccess);
        Assert.AreEqual(2, repository.GetProductsCalls);
    }

    [TestMethod]
    public async Task GetProducts_ReturnsFreshFailureResponse_WhenRepositoryThrows()
    {
        var repository = new StubProductRepository
        {
            GetProductsHandler = (_, _) => throw new InvalidOperationException("repository failure")
        };
        var service = new ProductService(new StubUnitOfWork(repository));

        var result = await service.GetProducts();

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("repository failure", result.Message);
        Assert.IsNull(result.Data);
        Assert.AreEqual(1, repository.GetProductsCalls);
    }

    [TestMethod]
    public void Constructor_HasNoDirectProductRepositoryDependency()
    {
        var constructors = typeof(ProductService).GetConstructors();

        Assert.AreEqual(1, constructors.Length);
        var parameterTypes = constructors[0].GetParameters().Select(parameter => parameter.ParameterType).ToArray();
        CollectionAssert.AreEqual(
            new[] { typeof(IUnitOfWork) },
            parameterTypes);
    }

    private static Producto Product(int id, string name) =>
        new()
        {
            idProducto = id,
            nombre = name,
            marca = "Test",
            destacado = 1,
            precio = 100m,
            stock = 1
        };

    private sealed class StubUnitOfWork(IProductRepository products) : IUnitOfWork
    {
        public IProductRepository Products { get; } = products;
        public IOrderRepository Orders => throw new NotSupportedException();
        public ISaleRepository Sales => throw new NotSupportedException();
        public ICategoriaRepository Categories => throw new NotSupportedException();
        public IProductCategoryRepository ProductCategories => throw new NotSupportedException();
        public ICartRepository Carts => throw new NotSupportedException();
        public IEnvioRepository Envio => throw new NotSupportedException();

        public Task<IDbContextTransaction> BeginTransactionAsync() => throw new NotSupportedException();
        public Task<int> SaveChangesAsync() => Task.FromResult(0);
        public void Dispose() { }
    }

    private sealed class StubProductRepository : IProductRepository
    {
        public Func<string?, string?, Task<IEnumerable<Producto>>>? GetProductsHandler { get; init; }
        public Func<Task<List<Producto>>>? GetFeaturedProductsHandler { get; init; }
        public int GetProductsCalls { get; private set; }
        public int GetFeaturedProductsCalls { get; private set; }

        public Task<IEnumerable<Producto>> GetProductsAsync(string? orden, string? precio)
        {
            GetProductsCalls++;
            return GetProductsHandler?.Invoke(orden, precio)
                ?? Task.FromResult<IEnumerable<Producto>>([]);
        }

        public Task<List<Producto>> GetFeaturedProductsAsync()
        {
            GetFeaturedProductsCalls++;
            return GetFeaturedProductsHandler?.Invoke()
                ?? Task.FromResult(new List<Producto>());
        }

        public Task<Producto?> GetProductByIdAsync(int id) => throw new NotSupportedException();
        public Task<IEnumerable<Producto>> GetProductsByTitleAsync(string nombre) => throw new NotSupportedException();
        public Task<Producto> AddProductAsync(Producto producto) => throw new NotSupportedException();
        public Task<bool> UpdateProductAsync(Producto producto) => throw new NotSupportedException();
        public Task<bool> DeleteProductAsync(int id) => throw new NotSupportedException();
        public Task<IEnumerable<Producto>> GetProductsByCategoryIdAsync(int categoriaId) => throw new NotSupportedException();
        public Task<IEnumerable<Producto>> ImportProductsFromExcelAsync(Stream fileStream) => throw new NotSupportedException();
    }
}
