using antigal.server.Models;
using antigal.server.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync();  // <- Método declarado aquí
        Task ConfirmOrder(OrderDto orderDto);   // Método para confirmar pedido
    }
}
