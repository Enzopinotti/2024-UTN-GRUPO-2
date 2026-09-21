using antigal.server.Models;
using antigal.server.Repositories;
using antigal.server.Services;
using Microsoft.EntityFrameworkCore.Storage;

namespace antigal.server.Tests;

[TestClass]
public class LikeServiceRepositoryAuthorityTests
{
    [TestMethod]
    public async Task AddLike_DelegatesToUnitOfWorkLikes()
    {
        var repository = new StubLikeRepository { AddResult = true };
        var service = new LikeService(new StubUnitOfWork(repository));

        var result = await service.AddLike("user-1", 10);

        Assert.IsTrue(result);
        Assert.AreEqual(1, repository.AddCalls);
        Assert.AreEqual(("user-1", 10), repository.LastAdd);
    }

    [TestMethod]
    public async Task RemoveLike_DelegatesToUnitOfWorkLikes()
    {
        var repository = new StubLikeRepository { RemoveResult = false };
        var service = new LikeService(new StubUnitOfWork(repository));

        var result = await service.RemoveLike("user-2", 20);

        Assert.IsFalse(result);
        Assert.AreEqual(1, repository.RemoveCalls);
        Assert.AreEqual(("user-2", 20), repository.LastRemove);
    }

    [TestMethod]
    public async Task GetUserLikes_DelegatesToUnitOfWorkLikes_AndPreservesList()
    {
        var products = new List<Producto>
        {
            new() { idProducto = 1, nombre = "A", marca = "M", precio = 10m, stock = 1 },
            new() { idProducto = 2, nombre = "B", marca = "M", precio = 20m, stock = 2 },
        };
        var repository = new StubLikeRepository { UserLikes = products };
        var service = new LikeService(new StubUnitOfWork(repository));

        var result = await service.GetUserLikes("user-3");

        Assert.AreSame(products, result);
        Assert.AreEqual(1, repository.GetCalls);
        Assert.AreEqual("user-3", repository.LastGetUserId);
    }

    [TestMethod]
    public void Constructor_HasOnlyUnitOfWorkDependency()
    {
        var constructors = typeof(LikeService).GetConstructors();

        Assert.AreEqual(1, constructors.Length);
        CollectionAssert.AreEqual(
            new[] { typeof(IUnitOfWork) },
            constructors[0].GetParameters().Select(parameter => parameter.ParameterType).ToArray());
    }

    private sealed class StubLikeRepository : ILikeRepository
    {
        public bool AddResult { get; init; }
        public bool RemoveResult { get; init; }
        public List<Producto> UserLikes { get; init; } = [];
        public int AddCalls { get; private set; }
        public int RemoveCalls { get; private set; }
        public int GetCalls { get; private set; }
        public (string UserId, int ProductoId)? LastAdd { get; private set; }
        public (string UserId, int ProductoId)? LastRemove { get; private set; }
        public string? LastGetUserId { get; private set; }

        public Task<bool> AddLikeAsync(string userId, int productoId)
        {
            AddCalls++;
            LastAdd = (userId, productoId);
            return Task.FromResult(AddResult);
        }

        public Task<bool> RemoveLikeAsync(string userId, int productoId)
        {
            RemoveCalls++;
            LastRemove = (userId, productoId);
            return Task.FromResult(RemoveResult);
        }

        public Task<List<Producto>> GetUserLikesAsync(string userId)
        {
            GetCalls++;
            LastGetUserId = userId;
            return Task.FromResult(UserLikes);
        }
    }

    private sealed class StubUnitOfWork(ILikeRepository likes) : IUnitOfWork
    {
        public ILikeRepository Likes { get; } = likes;
        public IContactoRepository Contactos => throw new NotSupportedException();
        public IImageRepository Images => throw new NotSupportedException();
        public IOrderRepository Orders => throw new NotSupportedException();
        public ISaleRepository Sales => throw new NotSupportedException();
        public IProductRepository Products => throw new NotSupportedException();
        public ICategoriaRepository Categories => throw new NotSupportedException();
        public IProductCategoryRepository ProductCategories => throw new NotSupportedException();
        public ICartRepository Carts => throw new NotSupportedException();
        public IEnvioRepository Envio => throw new NotSupportedException();

        public Task<IDbContextTransaction> BeginTransactionAsync() => throw new NotSupportedException();
        public void Dispose() { }
    }
}
