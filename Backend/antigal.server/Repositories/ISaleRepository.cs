using antigal.server.Models;

namespace antigal.server.Repositories
{
    public interface ISaleRepository
    {
        Task<Sale?> CreateSaleAsync(Sale sale);
        Task<Sale?> GetSaleByIdAsync(int idVenta);
        Task<bool> UpdateSaleAsync(Sale sale);
    }
}
