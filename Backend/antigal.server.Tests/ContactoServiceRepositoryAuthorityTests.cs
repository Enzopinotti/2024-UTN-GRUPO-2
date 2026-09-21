using antigal.server.Models;
using antigal.server.Repositories;
using antigal.server.Services;
using Microsoft.EntityFrameworkCore.Storage;

namespace antigal.server.Tests;

[TestClass]
public class ContactoServiceRepositoryAuthorityTests
{
    [TestMethod]
    public async Task CreateAsync_AssignsFecha_AndDelegatesToUnitOfWorkContactos()
    {
        var repository = new StubContactoRepository();
        var service = new ContactoService(new StubUnitOfWork(repository));
        var contacto = Contacto();
        var before = DateTime.Now;

        var result = await service.CreateAsync(contacto);

        var after = DateTime.Now;
        Assert.AreSame(contacto, result);
        Assert.AreSame(contacto, repository.LastAdded);
        Assert.AreEqual(1, repository.AddCalls);
        Assert.IsTrue(contacto.Fecha >= before);
        Assert.IsTrue(contacto.Fecha <= after);
    }

    [TestMethod]
    public async Task GetAllAsync_DelegatesAndPreservesRepositoryList()
    {
        var contactos = new List<Contacto> { Contacto(), Contacto("Segundo") };
        var repository = new StubContactoRepository { All = contactos };
        var service = new ContactoService(new StubUnitOfWork(repository));

        var result = await service.GetAllAsync();

        Assert.AreSame(contactos, result);
        Assert.AreEqual(1, repository.GetAllCalls);
    }

    [TestMethod]
    public async Task GetByIdAsync_DelegatesId_AndPreservesResult()
    {
        var contacto = Contacto();
        contacto.Id = 42;
        var repository = new StubContactoRepository { ById = contacto };
        var service = new ContactoService(new StubUnitOfWork(repository));

        var result = await service.GetByIdAsync(42);

        Assert.AreSame(contacto, result);
        Assert.AreEqual(1, repository.GetByIdCalls);
        Assert.AreEqual(42, repository.LastId);
    }

    [TestMethod]
    public void Constructor_HasOnlyUnitOfWorkDependency()
    {
        var constructors = typeof(ContactoService).GetConstructors();

        Assert.AreEqual(1, constructors.Length);
        CollectionAssert.AreEqual(
            new[] { typeof(IUnitOfWork) },
            constructors[0].GetParameters().Select(parameter => parameter.ParameterType).ToArray());
    }

    private static Contacto Contacto(string name = "Test") =>
        new()
        {
            Name = name,
            Email = "test@example.com",
            Asunto = "Consulta",
            Mensaje = "Mensaje"
        };

    private sealed class StubContactoRepository : IContactoRepository
    {
        public Contacto? LastAdded { get; private set; }
        public int AddCalls { get; private set; }
        public int GetAllCalls { get; private set; }
        public int GetByIdCalls { get; private set; }
        public int? LastId { get; private set; }
        public List<Contacto> All { get; init; } = [];
        public Contacto? ById { get; init; }

        public Task<Contacto> AddAsync(Contacto contacto)
        {
            AddCalls++;
            LastAdded = contacto;
            return Task.FromResult(contacto);
        }

        public Task<List<Contacto>> GetAllAsync()
        {
            GetAllCalls++;
            return Task.FromResult(All);
        }

        public Task<Contacto?> GetByIdAsync(int id)
        {
            GetByIdCalls++;
            LastId = id;
            return Task.FromResult(ById);
        }
    }

    private sealed class StubUnitOfWork(IContactoRepository contactos) : IUnitOfWork
    {
        public IContactoRepository Contactos { get; } = contactos;
        public IImageRepository Images => throw new NotSupportedException();
        public IOrderRepository Orders => throw new NotSupportedException();
        public ISaleRepository Sales => throw new NotSupportedException();
        public IProductRepository Products => throw new NotSupportedException();
        public ICategoriaRepository Categories => throw new NotSupportedException();
        public IProductCategoryRepository ProductCategories => throw new NotSupportedException();
        public ICartRepository Carts => throw new NotSupportedException();
        public ILikeRepository Likes => throw new NotSupportedException();
        public IEnvioRepository Envio => throw new NotSupportedException();

        public Task<IDbContextTransaction> BeginTransactionAsync() => throw new NotSupportedException();
        public void Dispose() { }
    }
}
