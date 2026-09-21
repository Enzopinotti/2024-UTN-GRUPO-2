using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Models.Dto.VentaDtos;
using antigal.server.Repositories;
using antigal.server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace antigal.server.Tests;

[TestClass]
public class OrderSalePersistenceAuthorityTests
{
    [TestMethod]
    public async Task UpdateSaleStatus_RepositoryOwnsPersistence_WithoutSecondUnitOfWorkSave()
    {
        var order = Order(10, "user-1");
        var sale = new Sale
        {
            idVenta = 20,
            idOrden = order.idOrden,
            Orden = order,
            total = 100m,
            metodoPago = "test",
            EstadoVenta = VentaEstado.Pendiente,
            idUsuario = order.idUsuario
        };
        var sales = new StubSaleRepository
        {
            GetSaleByIdHandler = _ => Task.FromResult<Sale?>(sale),
            UpdateSaleHandler = _ => Task.FromResult(true)
        };
        var unit = new StubUnitOfWork(
            new StubOrderRepository(),
            sales,
            new StubProductRepository(),
            new TrackingTransaction());
        var service = new SaleService(unit);

        var result = await service.UpdateSaleStatusAsync(sale.idVenta, VentaEstado.Completada);

        Assert.IsTrue(result);
        Assert.AreEqual(VentaEstado.Completada, sale.EstadoVenta);
        Assert.AreEqual(1, sales.GetSaleByIdCalls);
        Assert.AreEqual(1, sales.UpdateSaleCalls);
        Assert.AreEqual(0, unit.SaveChangesCalls);
    }

    [TestMethod]
    public async Task ConfirmOrder_CommitsTransaction_AfterRepositoryOwnedMutations_WithoutFinalFlush()
    {
        var product = Product(30, "Product", stock: 10, price: 50m);
        var order = Order(40, "user-1");
        order.Items.Add(new OrdenDetalle
        {
            idDetalle = 1,
            idOrdenDetalle = order.idOrden,
            idProducto = product.idProducto,
            cantidad = 2,
            precio = product.precio,
            Orden = order,
            Producto = product
        });

        var orders = new StubOrderRepository
        {
            GetPendingHandler = _ => Task.FromResult<Orden?>(order),
            UpdateStatusHandler = (_, _) => Task.FromResult(true)
        };
        var sales = new StubSaleRepository
        {
            CreateSaleHandler = sale =>
            {
                sale.idVenta = 50;
                return Task.FromResult<Sale?>(sale);
            }
        };
        var products = new StubProductRepository
        {
            UpdateProductHandler = _ => Task.FromResult(true)
        };
        var transaction = new TrackingTransaction();
        var unit = new StubUnitOfWork(orders, sales, products, transaction);
        var service = new OrderService(
            unit,
            new StubUserManager(new User { Id = order.idUsuario, UserName = "user" }),
            new StubProductService(product));

        await service.ConfirmOrder(new OrdenDto
        {
            idUsuario = order.idUsuario,
            Items = [new OrdenDetalleDto { idProducto = product.idProducto, cantidad = 2 }]
        });

        Assert.AreEqual("Confirmada", order.estado);
        Assert.AreEqual(8, product.stock);
        Assert.AreEqual(1, orders.UpdateStatusCalls);
        Assert.AreEqual(1, sales.CreateSaleCalls);
        Assert.AreEqual(1, products.UpdateProductCalls);
        Assert.AreEqual(0, unit.SaveChangesCalls);
        Assert.AreEqual(1, transaction.CommitCalls);
        Assert.AreEqual(0, transaction.RollbackCalls);
    }

    [TestMethod]
    public async Task ConfirmOrder_RollsBackTransaction_WhenStockIsInsufficient()
    {
        var product = Product(60, "Low stock", stock: 1, price: 25m);
        var order = Order(70, "user-2");
        order.Items.Add(new OrdenDetalle
        {
            idDetalle = 2,
            idOrdenDetalle = order.idOrden,
            idProducto = product.idProducto,
            cantidad = 2,
            precio = product.precio,
            Orden = order,
            Producto = product
        });

        var orders = new StubOrderRepository
        {
            GetPendingHandler = _ => Task.FromResult<Orden?>(order)
        };
        var sales = new StubSaleRepository();
        var products = new StubProductRepository();
        var transaction = new TrackingTransaction();
        var unit = new StubUnitOfWork(orders, sales, products, transaction);
        var service = new OrderService(
            unit,
            new StubUserManager(new User { Id = order.idUsuario, UserName = "user" }),
            new StubProductService(product));

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
            service.ConfirmOrder(new OrdenDto
            {
                idUsuario = order.idUsuario,
                Items = [new OrdenDetalleDto { idProducto = product.idProducto, cantidad = 2 }]
            }));

        Assert.AreEqual("Pendiente", order.estado);
        Assert.AreEqual(1, product.stock);
        Assert.AreEqual(0, orders.UpdateStatusCalls);
        Assert.AreEqual(0, sales.CreateSaleCalls);
        Assert.AreEqual(0, products.UpdateProductCalls);
        Assert.AreEqual(0, unit.SaveChangesCalls);
        Assert.AreEqual(0, transaction.CommitCalls);
        Assert.AreEqual(1, transaction.RollbackCalls);
    }

    private static Producto Product(int id, string name, int stock, decimal price) =>
        new()
        {
            idProducto = id,
            nombre = name,
            marca = "Test",
            stock = stock,
            precio = price,
            disponible = stock > 0 ? 1 : 0
        };

    private static Orden Order(int id, string userId) =>
        new()
        {
            idOrden = id,
            idUsuario = userId,
            estado = "Pendiente",
            fechaOrden = DateTime.UtcNow
        };

    private sealed class StubUnitOfWork(
        IOrderRepository orders,
        ISaleRepository sales,
        IProductRepository products,
        TrackingTransaction transaction) : IUnitOfWork
    {
        public IOrderRepository Orders { get; } = orders;
        public ISaleRepository Sales { get; } = sales;
        public IProductRepository Products { get; } = products;
        public ICategoriaRepository Categories => throw new NotSupportedException();
        public IProductCategoryRepository ProductCategories => throw new NotSupportedException();
        public ICartRepository Carts => throw new NotSupportedException();
        public IEnvioRepository Envio => throw new NotSupportedException();
        public int SaveChangesCalls { get; private set; }

        public Task<IDbContextTransaction> BeginTransactionAsync() =>
            Task.FromResult<IDbContextTransaction>(transaction);

        public Task<int> SaveChangesAsync()
        {
            SaveChangesCalls++;
            return Task.FromResult(0);
        }

        public void Dispose() { }
    }

    private sealed class StubOrderRepository : IOrderRepository
    {
        public Func<string, Task<Orden?>>? GetPendingHandler { get; init; }
        public Func<int, string, Task<bool>>? UpdateStatusHandler { get; init; }
        public int UpdateStatusCalls { get; private set; }

        public Task<Orden?> GetPendingOrderByUserIdAsync(string userId) =>
            GetPendingHandler?.Invoke(userId) ?? Task.FromResult<Orden?>(null);

        public Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            UpdateStatusCalls++;
            return UpdateStatusHandler?.Invoke(orderId, newStatus) ?? Task.FromResult(true);
        }

        public Task<List<Orden>> GetAllOrdersAsync() => throw new NotSupportedException();
        public Task<List<Orden>> GetOrdersByUserIdAsync(string userId) => throw new NotSupportedException();
        public Task<List<Orden>> GetOrdersByStatusAsync(string status) => throw new NotSupportedException();
        public Task<Orden> GetOrderByIdAsync(int orderId) => throw new NotSupportedException();
        public Task AddOrderAsync(Orden order) => throw new NotSupportedException();
    }

    private sealed class StubSaleRepository : ISaleRepository
    {
        public Func<Sale, Task<Sale?>>? CreateSaleHandler { get; init; }
        public Func<int, Task<Sale?>>? GetSaleByIdHandler { get; init; }
        public Func<Sale, Task<bool>>? UpdateSaleHandler { get; init; }
        public int CreateSaleCalls { get; private set; }
        public int GetSaleByIdCalls { get; private set; }
        public int UpdateSaleCalls { get; private set; }

        public Task<Sale?> CreateSaleAsync(Sale sale)
        {
            CreateSaleCalls++;
            return CreateSaleHandler?.Invoke(sale) ?? Task.FromResult<Sale?>(sale);
        }

        public Task<Sale?> GetSaleByIdAsync(int idVenta)
        {
            GetSaleByIdCalls++;
            return GetSaleByIdHandler?.Invoke(idVenta) ?? Task.FromResult<Sale?>(null);
        }

        public Task<bool> UpdateSaleAsync(Sale sale)
        {
            UpdateSaleCalls++;
            return UpdateSaleHandler?.Invoke(sale) ?? Task.FromResult(true);
        }
    }

    private sealed class StubProductRepository : IProductRepository
    {
        public Func<Producto, Task<bool>>? UpdateProductHandler { get; init; }
        public int UpdateProductCalls { get; private set; }

        public Task<bool> UpdateProductAsync(Producto producto)
        {
            UpdateProductCalls++;
            return UpdateProductHandler?.Invoke(producto) ?? Task.FromResult(true);
        }

        public Task<IEnumerable<Producto>> GetProductsAsync(string? orden, string? precio) => throw new NotSupportedException();
        public Task<Producto?> GetProductByIdAsync(int id) => throw new NotSupportedException();
        public Task<IEnumerable<Producto>> GetProductsByTitleAsync(string nombre) => throw new NotSupportedException();
        public Task<Producto> AddProductAsync(Producto producto) => throw new NotSupportedException();
        public Task<bool> DeleteProductAsync(int id) => throw new NotSupportedException();
        public Task<IEnumerable<Producto>> GetProductsByCategoryIdAsync(int categoriaId) => throw new NotSupportedException();
        public Task<IEnumerable<Producto>> ImportProductsFromExcelAsync(Stream fileStream) => throw new NotSupportedException();
        public Task<List<Producto>> GetFeaturedProductsAsync() => throw new NotSupportedException();
    }

    private sealed class StubProductService(Producto product) : IProductService
    {
        public Task<ResponseDto> GetProductByIdAsync(int id) =>
            Task.FromResult(new ResponseDto
            {
                IsSuccess = id == product.idProducto,
                Data = id == product.idProducto ? product : null
            });

        public Task<ResponseDto> GetProducts(string? orden = null, string? precio = null) => throw new NotSupportedException();
        public Task<ResponseDto> GetProductByTitleAsync(string nombre) => throw new NotSupportedException();
        public Task<ResponseDto> AddProductAsync(Producto producto) => throw new NotSupportedException();
        public Task<ResponseDto> DeleteProductAsync(int id) => throw new NotSupportedException();
        public Task<ResponseDto> PutProductAsync(Producto producto) => throw new NotSupportedException();
        public Task<ResponseDto> ImportProductsFromExcelAsync(IFormFile file) => throw new NotSupportedException();
        public Task<ResponseDto> GetProductsByCategoryIdAsync(int categoriaId) => throw new NotSupportedException();
        public Task<ResponseDto> GetProductsHomeAsync() => throw new NotSupportedException();
    }

    private sealed class StubUserManager(User user) : UserManager<User>(
        new StubUserStore(),
        Options.Create(new IdentityOptions()),
        new PasswordHasher<User>(),
        Array.Empty<IUserValidator<User>>(),
        Array.Empty<IPasswordValidator<User>>(),
        new UpperInvariantLookupNormalizer(),
        new IdentityErrorDescriber(),
        null,
        NullLogger<UserManager<User>>.Instance)
    {
        public override Task<User?> FindByIdAsync(string userId) =>
            Task.FromResult<User?>(user.Id == userId ? user : null);
    }

    private sealed class StubUserStore : IUserStore<User>
    {
        public void Dispose() { }
        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.Id);
        public Task<string?> GetUserNameAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);
        public Task SetUserNameAsync(User user, string? userName, CancellationToken cancellationToken) { user.UserName = userName; return Task.CompletedTask; }
        public Task<string?> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.NormalizedUserName);
        public Task SetNormalizedUserNameAsync(User user, string? normalizedName, CancellationToken cancellationToken) { user.NormalizedUserName = normalizedName; return Task.CompletedTask; }
        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
        public Task<User?> FindByIdAsync(string userId, CancellationToken cancellationToken) => Task.FromResult<User?>(null);
        public Task<User?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) => Task.FromResult<User?>(null);
    }

    private sealed class TrackingTransaction : IDbContextTransaction
    {
        public Guid TransactionId { get; } = Guid.NewGuid();
        public bool SupportsSavepoints => false;
        public int CommitCalls { get; private set; }
        public int RollbackCalls { get; private set; }

        public void Commit() => CommitCalls++;
        public Task CommitAsync(CancellationToken cancellationToken = default) { CommitCalls++; return Task.CompletedTask; }
        public void Rollback() => RollbackCalls++;
        public Task RollbackAsync(CancellationToken cancellationToken = default) { RollbackCalls++; return Task.CompletedTask; }
        public void CreateSavepoint(string name) => throw new NotSupportedException();
        public Task CreateSavepointAsync(string name, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public void RollbackToSavepoint(string name) => throw new NotSupportedException();
        public Task RollbackToSavepointAsync(string name, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public void ReleaseSavepoint(string name) => throw new NotSupportedException();
        public Task ReleaseSavepointAsync(string name, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public void Dispose() { }
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
