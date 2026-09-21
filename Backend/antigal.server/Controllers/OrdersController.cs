using antigal.server.Models.Dto;
using antigal.server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace antigal.server.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Obtener todas las órdenes
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        /// <summary>
        /// Confirmar una orden y crear una venta asociada.
        /// </summary>
        /// <param name="orderId">ID de la orden a confirmar.</param>
        [HttpPost("confirm/{orderId}")]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null)
                {
                    return NotFound(new { Message = "Orden no encontrada" });
                }

                var orderDto = new OrdenDto
                {
                    idUsuario = order.idUsuario,
                    Items = order.Items.Select(item => new OrdenDetalleDto
                    {
                        idProducto = item.idProducto,
                        cantidad = item.cantidad
                    }).ToList(),
                    montoTotal = order.Items.Sum(item => item.cantidad * (item.Producto?.precio ?? 0))
                };

                await _orderService.ConfirmOrder(orderDto);

                return Ok(new { Message = "Orden confirmada y venta creada con éxito." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error al confirmar la orden", Details = ex.Message });
            }
        }
    }
}
