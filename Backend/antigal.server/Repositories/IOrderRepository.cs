using antigal.server.Models;

namespace antigal.server.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Orden>> GetAllOrdersAsync();
        Task<Orden> GetOrderByIdAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<Orden?> GetPendingOrderByUserIdAsync(string userId);
    }
}
