using Microsoft.EntityFrameworkCore;
using antigal.server.Data;
using antigal.server.Models;
using System.Threading.Tasks;

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

        public async Task<Sale?> GetSaleByIdAsync(int idVenta)
        {
            try
            {
                return await _context.Sales
                                     .Include(s => s.Orden)
                                     .FirstOrDefaultAsync(s => s.idVenta == idVenta);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al intentar obtener la venta de la base de datos", ex);
            }
        }

        public async Task<bool> UpdateSaleAsync(Sale sale)
        {
            _context.Sales.Update(sale);
            await _context.SaveChangesAsync();
            return true;
        }

    }

}
