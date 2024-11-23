using antigal.server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace antigal.server.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Orden>> GetAllOrdersAsync(); // Obtener todas las órdenes
        Task<List<Orden>> GetOrdersByUserIdAsync(string idUsuario); // Obtener órdenes por idUsuario
        Task<List<Orden>> GetOrdersByStatusAsync(string status); // Obtener órdenes por estado
        Task<Orden> GetOrderByIdAsync(int orderId);
        Task AddOrderAsync(Orden orden); // Agregar una nueva orden
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus); // Actualizar estado de una orden
        Task<Orden?> GetPendingOrderByUserIdAsync(string userId); // Obtener una orden "Pending" de un usuario

    }
}
