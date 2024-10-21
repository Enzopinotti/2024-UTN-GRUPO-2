using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Repositories;

namespace antigal.server.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public ResponseDto GetCartByUserId(string userId)
        {
            return _cartRepository.GetCartByUserId(userId);
        }

        public ResponseDto CreateCart(string userId)
        {
            return _cartRepository.CreateCart(userId);
        }

        public ResponseDto AddItemToCart(string userId, CarritoItem item)
        {
            return _cartRepository.AddItemToCart(userId, item);
        }

        public ResponseDto RemoveItemFromCart(string userId, int itemId)
        {
            return _cartRepository.RemoveItemFromCart(userId, itemId);
        }

        public ResponseDto ClearCart(string userId)
        {
            return _cartRepository.ClearCart(userId);
        }
    }
}
