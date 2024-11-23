using antigal.server.Models;
using antigal.server.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public interface IOrderService
    {
        Task<List<Orden>> GetAllOrdersAsync();  // Obtener todas las órdenes
        Task<Orden?> GetOrderByIdAsync(int orderId); // Obtener una orden específica por ID
        Task<List<Orden>> GetOrdersByUserIdAsync(string userId); // Obtener órdenes por usuario
        Task<List<Orden>> GetOrdersByStatusAsync(string status); // Obtener órdenes por estado
        Task<Orden?> GetPendingOrderByUserIdAsync(string userId); // Obtener una orden pendiente por usuario
        Task ConfirmOrder(OrdenDto orderDto);   // Confirmar una orden
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus); // Actualizar estado de la orden
    }
}

