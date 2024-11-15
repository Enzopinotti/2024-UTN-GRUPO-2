// Repositories/IUnitOfWork.cs
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace antigal.server.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IOrderRepository Orders { get; }
        ISaleRepository Sales { get; }
        IProductRepository Products { get; }
        ICategoriaRepository Categories { get; }
        IProductCategoryRepository ProductCategories { get; }
        ICartRepository Carts { get; }
        IEnvioRepository Envio { get; }

        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<int> SaveChangesAsync();
    }
}