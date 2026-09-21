using antigal.server.Models;
using antigal.server.Models.Dto;

namespace antigal.server.Services
{
    public interface IOrderService
    {
        Task<List<Orden>> GetAllOrdersAsync();
        Task<Orden?> GetOrderByIdAsync(int orderId);
        Task ConfirmOrder(OrdenDto orderDto);
    }
}
