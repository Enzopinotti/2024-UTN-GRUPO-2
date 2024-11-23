// Services/CartService.cs
using antigal.server.Models.Dto;
using antigal.server.Models.Dto.CarritoDtos;
using antigal.server.Repositories;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto> GetCartByUserIdAsync(string userId)
        {
            return await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
        }

        public async Task<ResponseDto> CreateCartAsync(string userId)
        {
            return await _unitOfWork.Carts.CreateCartAsync(userId);
        }

        public async Task<ResponseDto> AddItemToCartAsync(string userId, CarritoItemDto addItemDto)
        {
            return await _unitOfWork.Carts.AddItemToCartAsync(userId, addItemDto);
        }

        public async Task<ResponseDto> RemoveItemFromCartAsync(string userId, int itemId)
        {
            return await _unitOfWork.Carts.RemoveItemFromCartAsync(userId, itemId);
        }

        public async Task<ResponseDto> ClearCartAsync(string userId)
        {
            return await _unitOfWork.Carts.ClearCartAsync(userId);
        }

        public async Task<ResponseDto> ConfirmCartAsOrderAsync(string userId)
        {
            return await _unitOfWork.Carts.ConfirmCartAsOrderAsync(userId);
        }
    }
}