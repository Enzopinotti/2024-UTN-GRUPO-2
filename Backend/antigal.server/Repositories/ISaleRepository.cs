using antigal.server.Models;

namespace antigal.server.Repositories
{
    public interface ISaleRepository
    {
        Task<Sale?> CreateSaleAsync(Sale sale);
    }
}
