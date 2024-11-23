// Repositories/ICartRepository.cs
using antigal.server.Models.Dto;
using System.Threading.Tasks;

namespace antigal.server.Repositories
{
    public interface ICartRepository
    {
        Task<ResponseDto> GetCartByUserIdAsync(string userId);
        Task<ResponseDto> CreateCartAsync(string userId);
        Task<ResponseDto> AddItemToCartAsync(string userId, CarritoItemDto addItemDto);
        Task<ResponseDto> RemoveItemFromCartAsync(string userId, int itemId);
        Task<ResponseDto> ClearCartAsync(string userId);
        Task<ResponseDto> ConfirmCartAsOrderAsync(string userId);
    }
}