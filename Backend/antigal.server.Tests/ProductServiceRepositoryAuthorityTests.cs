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
    public async Task AddProduct_EmptyTitleMatch_AddsOnce_ThroughRepositoryPersistence()
    {
        var product = Product(30, "New product");
        var repository = new StubProductRepository
        {
            GetProductsByTitleHandler = _ => Task.FromResult<IEnumerable<Producto>>([]),
            AddProductHandler = candidate => Task.FromResult(candidate)
        };
        var unitOfWork = new StubUnitOfWork(repository);
        var service = new ProductService(unitOfWork);

        var result = await service.AddProductAsync(product);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Producto agregado exitosamente.", result.Message);
        Assert.AreSame(product, result.Data);
        Assert.AreEqual(1, repository.GetProductsByTitleCalls);
        Assert.AreEqual(1, repository.AddProductCalls);
    }

    [TestMethod]
    public async Task AddProduct_ExistingTitle_DoesNotAdd()
    {
        var product = Product(31, "Existing product");
        var repository = new StubProductRepository
        {
            GetProductsByTitleHandler = _ =>
                Task.FromResult<IEnumerable<Producto>>([Product(99, "Existing product")])
        };
        var unitOfWork = new StubUnitOfWork(repository);
        var service = new ProductService(unitOfWork);

        var result = await service.AddProductAsync(product);

        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Ya existe un producto con ese nombre.", result.Message);
        Assert.AreEqual(1, repository.GetProductsByTitleCalls);
        Assert.AreEqual(0, repository.AddProductCalls);
    }

    [TestMethod]
    public async Task DeleteProduct_ExistingProduct_DelegatesRepositoryPersistence()
    {
        var existing = Product(40, "Delete me");
        var repository = new StubProductRepository
        {
            GetProductByIdHandler = _ => Task.FromResult<Producto?>(existing),
            DeleteProductHandler = _ => Task.FromResult(true)
        };
        var unitOfWork = new StubUnitOfWork(repository);
        var service = new ProductService(unitOfWork);

        var result = await service.DeleteProductAsync(existing.idProducto);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Producto eliminado exitosamente.", result.Message);
        Assert.AreEqual(1, repository.GetProductByIdCalls);
        Assert.AreEqual(1, repository.DeleteProductCalls);
    }

    [TestMethod]
    public async Task PutProduct_ExistingProduct_DelegatesRepositoryPersistence()
    {
        var existing = Product(50, "Old");
        existing.descripcion = "old description";
        existing.precio = 10m;
        existing.stock = 2;

        var update = Product(50, "Updated");
        update.descripcion = "new description";
        update.precio = 25m;
        update.stock = 8;

        var repository = new StubProductRepository
        {
            GetProductByIdHandler = _ => Task.FromResult<Producto?>(existing),
            UpdateProductHandler = _ => Task.FromResult(true)
        };
        var unitOfWork = new StubUnitOfWork(repository);
        var service = new ProductService(unitOfWork);

        var result = await service.PutProductAsync(update);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Producto actualizado exitosamente.", result.Message);
        Assert.AreSame(existing, result.Data);
        Assert.AreEqual("Updated", existing.nombre);
        Assert.AreEqual("new description", existing.descripcion);
        Assert.AreEqual(25m, existing.precio);
        Assert.AreEqual(8, existing.stock);
        Assert.AreEqual(1, repository.GetProductByIdCalls);
        Assert.AreEqual(1, repository.UpdateProductCalls);
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

        public void Dispose() { }
    }

    private sealed class StubProductRepository : IProductRepository
    {
        public Func<string?, string?, Task<IEnumerable<Producto>>>? GetProductsHandler { get; init; }
        public Func<Task<List<Producto>>>? GetFeaturedProductsHandler { get; init; }
        public Func<string, Task<IEnumerable<Producto>>>? GetProductsByTitleHandler { get; init; }
        public Func<Producto, Task<Producto>>? AddProductHandler { get; init; }
        public Func<int, Task<Producto?>>? GetProductByIdHandler { get; init; }
        public Func<int, Task<bool>>? DeleteProductHandler { get; init; }
        public Func<Producto, Task<bool>>? UpdateProductHandler { get; init; }

        public int GetProductsCalls { get; private set; }
        public int GetFeaturedProductsCalls { get; private set; }
        public int GetProductsByTitleCalls { get; private set; }
        public int AddProductCalls { get; private set; }
        public int GetProductByIdCalls { get; private set; }
        public int DeleteProductCalls { get; private set; }
        public int UpdateProductCalls { get; private set; }

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

        public Task<Producto?> GetProductByIdAsync(int id)
        {
            GetProductByIdCalls++;
            return GetProductByIdHandler?.Invoke(id)
                ?? Task.FromResult<Producto?>(null);
        }

        public Task<IEnumerable<Producto>> GetProductsByTitleAsync(string nombre)
        {
            GetProductsByTitleCalls++;
            return GetProductsByTitleHandler?.Invoke(nombre)
                ?? Task.FromResult<IEnumerable<Producto>>([]);
        }

        public Task<Producto> AddProductAsync(Producto producto)
        {
            AddProductCalls++;
            return AddProductHandler?.Invoke(producto)
                ?? Task.FromResult(producto);
        }

        public Task<bool> UpdateProductAsync(Producto producto)
        {
            UpdateProductCalls++;
            return UpdateProductHandler?.Invoke(producto)
                ?? Task.FromResult(true);
        }

        public Task<bool> DeleteProductAsync(int id)
        {
            DeleteProductCalls++;
            return DeleteProductHandler?.Invoke(id)
                ?? Task.FromResult(true);
        }
        public Task<IEnumerable<Producto>> GetProductsByCategoryIdAsync(int categoriaId) => throw new NotSupportedException();
        public Task<IEnumerable<Producto>> ImportProductsFromExcelAsync(Stream fileStream) => throw new NotSupportedException();
    }
}
