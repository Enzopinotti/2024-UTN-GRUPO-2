using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();  // Implementación del método
        }

        public async Task ConfirmOrder(OrderDto orderDto)
        {
            var order = new Order
            {
                UserId = orderDto.UserId,
                OrderDate = DateTime.Now,
                Items = orderDto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList(),
                Status = "Pending"
            };

            await _orderRepository.AddOrderAsync(order);
        }


        }

    }

