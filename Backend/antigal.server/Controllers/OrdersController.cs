﻿// OrdersController.cs
using antigal.server.Models.Dto;
using antigal.server.Services;
using Microsoft.AspNetCore.Mvc;

namespace antigal.server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmOrder([FromBody] OrderDto orderDto)
        {
            await _orderService.ConfirmOrder(orderDto);
            return Ok("Pedido confirmado con éxito.");
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
    }
}