using antigal.server.Models.Dto;
using antigal.server.Models.Dto.CarritoDtos;
using antigal.server.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // Obtener el carrito de un usuario por su ID
        [HttpGet("{userId}")]
        public async Task<ActionResult<CarritoDto>> GetCartByUserIdAsync(string userId)
        {
            var response = await _cartService.GetCartByUserIdAsync(userId);
            if (!response.IsSuccess) return NotFound(new { response.Message });
            if (response.Data is CarritoDto carritoDto) return Ok(carritoDto);
            return StatusCode(500, "Error inesperado al recuperar el carrito.");
        }

        // Crear un carrito vacío para un usuario
        [HttpPost("{userId}")]
        public async Task<ActionResult<CarritoDto>> CreateCartAsync(string userId)
        {
            var response = await _cartService.CreateCartAsync(userId);
            if (!response.IsSuccess) return BadRequest(new { response.Message });
            return Ok(response);
        }

        // Agregar un ítem al carrito de un usuario
        [HttpPost("{userId}/items")]
        public async Task<ActionResult<CarritoItemDto>> AddItemToCartAsync(string userId, [FromBody] CarritoItemDto addItemDto)
        {
            var response = await _cartService.AddItemToCartAsync(userId, addItemDto);
            if (!response.IsSuccess) return BadRequest(new { response.Message });
            return Ok(response);
        }

        // Eliminar un ítem del carrito de un usuario
        [HttpDelete("{userId}/items/{itemId}")]
        public async Task<ActionResult<EliminarCarritoItemDto>> RemoveItemFromCartAsync(string userId, int itemId)
        {
            var response = await _cartService.RemoveItemFromCartAsync(userId, itemId);
            if (!response.IsSuccess) return BadRequest(new { response.Message });
            return Ok(response);
        }

        // Vaciar el carrito de un usuario
        [HttpDelete("{userId}/clear")]
        public async Task<ActionResult<VaciarCarritoDto>> ClearCartAsync(string userId)
        {
            var response = await _cartService.ClearCartAsync(userId);
            if (!response.IsSuccess) return BadRequest(new { response.Message });
            return Ok(response);
        }

        // Confirmar carrito como orden
        [HttpPost("{userId}/confirmar")]
        public async Task<IActionResult> ConfirmCartAsOrderAsync(string userId)
        {
            var response = await _cartService.ConfirmCartAsOrderAsync(userId);
            if (!response.IsSuccess) return BadRequest(new { response.Message });
            return Ok(new { Message = "Carrito confirmado como orden exitosamente.", Orden = response.Data });
        }
    }
}
