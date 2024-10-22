using antigal.server.Models.Dto;
using antigal.server.Models;
using antigal.server.Services;
using Microsoft.AspNetCore.Mvc;

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
        public ResponseDto GetCartByUserId(string userId)
        {
            return _cartService.GetCartByUserId(userId);
        }

        // Crear un carrito vacío para un usuario
        [HttpPost("{userId}")]
        public ResponseDto CreateCart(string userId)
        {
            return _cartService.CreateCart(userId);
        }

        // Agregar un ítem al carrito de un usuario
        [HttpPost("{userId}/items")]
        public ResponseDto AddItemToCart(string userId, [FromBody] AddItemToCartDto addItemDto)
        {
            return _cartService.AddItemToCart(userId, addItemDto);
        }

        // Eliminar un ítem del carrito de un usuario
        [HttpDelete("{userId}/items/{itemId}")]
        public ResponseDto RemoveItemFromCart(string userId, int itemId)
        {
            return _cartService.RemoveItemFromCart(userId, itemId);
        }

        // Vaciar el carrito de un usuario
        [HttpDelete("{userId}/clear")]
        public ResponseDto ClearCart(string userId)
        {
            return _cartService.ClearCart(userId);
        }
    }
}
