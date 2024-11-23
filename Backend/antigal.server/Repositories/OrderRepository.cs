using antigal.server.Data;
using antigal.server.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq; // Asegúrate de incluir esto para usar Where
using System.Threading.Tasks;

namespace antigal.server.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Orden>> GetAllOrdersAsync()
        {
            return await _context.Ordenes
                .Include(o => o.Items) // Asegúrate de incluir los ítems de la orden
                .ToListAsync();
        }

        public async Task<List<Orden>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Ordenes
                .Include(o => o.Items) // Incluye ítems de la orden
                .Where(o => o.idUsuario == userId) // Filtra por idUsuario
                .ToListAsync();
        }

        public async Task<List<Orden>> GetOrdersByStatusAsync(string status)
        {
            return await _context.Ordenes
                .Include(o => o.Items) // Incluye ítems de la orden si es necesario
                .Where(o => o.estado == status)
                .ToListAsync();
        }

        public async Task<Orden> GetOrderByIdAsync(int idOrden)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Items)
                .ThenInclude(od => od.Producto) // Incluye el producto de cada ítem
                .FirstOrDefaultAsync(o => o.idOrden == idOrden);

            if (orden == null)
            {
                throw new KeyNotFoundException($"No se encontró la orden con ID: {idOrden}.");
            }

            return orden;
        }


        public async Task AddOrderAsync(Orden order)
        {
            await _context.Ordenes.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var order = await _context.Ordenes.FindAsync(orderId);
            if (order == null) return false;

            order.estado = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Orden?> GetPendingOrderByUserIdAsync(string userId)
        {
            return await _context.Ordenes
                .Include(o => o.Items) // Incluye ítems de la orden si es necesario
                .FirstOrDefaultAsync(o => o.idUsuario == userId && o.estado == "Pendiente");
        }
    }
}
