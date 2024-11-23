// Services/ICartService.cs
using antigal.server.Models.Dto;
using antigal.server.Models.Dto.CarritoDtos;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public interface ICartService
    {
        Task<ResponseDto> GetCartByUserIdAsync(string userId);
        Task<ResponseDto> CreateCartAsync(string userId);
        Task<ResponseDto> AddItemToCartAsync(string userId, CarritoItemDto addItemDto);
        Task<ResponseDto> RemoveItemFromCartAsync(string userId, int itemId);
        Task<ResponseDto> ClearCartAsync(string userId);
        Task<ResponseDto> ConfirmCartAsOrderAsync(string userId);
    }
}