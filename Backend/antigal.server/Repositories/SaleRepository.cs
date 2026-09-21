using Microsoft.EntityFrameworkCore;
using antigal.server.Data;
using antigal.server.Models;

namespace antigal.server.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly AppDbContext _context;

        public SaleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Sale?> CreateSaleAsync(Sale sale)
        {
            try
            {
                await _context.Sales.AddAsync(sale);
                await _context.SaveChangesAsync();
                return sale;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error al intentar crear la venta en la base de datos", ex);
            }
        }
    }
}
