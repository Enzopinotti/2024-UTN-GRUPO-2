using antigal.server.Models;

namespace antigal.server.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetOrdersByStatusAsync(string status);
        Task AddOrderAsync(Order order);
        Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus);
    }
}
