/*using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAuthService _authService;
        private readonly IProductService _productService;

        public OrderService(IOrderRepository orderRepository, IAuthService authService, IProductService productService)
        {
            _orderRepository = orderRepository;
            _authService = authService;
            _productService = productService;

        }

        public async Task<List<Orden>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task<Orden?> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }

        public async Task<List<Orden>> GetOrdersByUserIdAsync(string userId)
        {
            return await _orderRepository.GetOrdersByUserIdAsync(userId);
        }

        public async Task<List<Orden>> GetOrdersByStatusAsync(string status)
        {
            return await _orderRepository.GetOrdersByStatusAsync(status);
        }

        public async Task ConfirmOrder(OrdenDto orderDto)
        {
            // Recupera el usuario por su ID
            var user = await _authService.GetUserByIdAsync(orderDto.idUsuario); // Asegúrate de que este método existe en tu repositorio

            // Verifica si el usuario existe
            if (user == null)
            {
                throw new KeyNotFoundException($"No se encontró el usuario con ID: {orderDto.idUsuario}.");
            }

            // Crea la orden
            var orden = new Orden
            {
                idUsuario = orderDto.idUsuario,
                fechaOrden = DateTime.Now,
                estado = "Pending",
                User = user, // Asigna el usuario recuperado
                Items = new List<OrdenDetalle>() // Inicializa la lista de items
            };

            // Agregar los items a la orden
            foreach (var i in orderDto.Items)
            {
                // Obtener el producto usando el servicio
                var response = await _productService.GetProductByIdAsync(i.idProducto); // Obtiene el ResponseDto

                // Verificar que la respuesta sea exitosa y obtener el producto
                if (!response.IsSuccess || response.Data == null)
                {
                    throw new KeyNotFoundException($"No se encontró el producto con ID: {i.idProducto}.");
                }

                var producto = (Producto)response.Data; // Asegúrate de que Data sea de tipo Producto

                // Crear un nuevo item y asignar la orden
                var ordenItem = new OrdenDetalle
                {
                    idProducto = i.idProducto,
                    cantidad = i.cantidad,
                    Producto = producto, // Asignar el producto
                    Orden = orden // Asignar la orden
                };

                // Agregar el item a la lista de items de la orden
                orden.Items.Add(ordenItem);
            }

            // Guardar la orden en el repositorio
            await _orderRepository.AddOrderAsync(orden);
        }



        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            return await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus);
        }
    }
}
*/