using antigal.server.Models.Dto;
using antigal.server.Models.Dto.CarritoDtos;
using antigal.server.Repositories;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<ResponseDto> GetCartByUserIdAsync(string userId)
        {
            return await _cartRepository.GetCartByUserIdAsync(userId);
        }

        public async Task<ResponseDto> CreateCartAsync(string userId)
        {
            return await _cartRepository.CreateCartAsync(userId);
        }

        public async Task<ResponseDto> AddItemToCartAsync(string userId, CarritoItemDto addItemDto)
        {
            return await _cartRepository.AddItemToCartAsync(userId, addItemDto);
        }

        public async Task<ResponseDto> RemoveItemFromCartAsync(string userId, int itemId)
        {
            return await _cartRepository.RemoveItemFromCartAsync(userId, itemId);
        }

        public async Task<ResponseDto> ClearCartAsync(string userId)
        {
            return await _cartRepository.ClearCartAsync(userId);
        }
    }
}
