using antigal.server.Models.Dto;
using antigal.server.Models.Dto.CarritoDtos;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public interface ICartService
    {
        Task<ResponseDto> GetCartByUserIdAsync(string userId);  // Obtener el carrito por UserId
        Task<ResponseDto> CreateCartAsync(string userId);  // Crear un carrito vacío para el usuario
        Task<ResponseDto> AddItemToCartAsync(string userId, CarritoItemDto addItemDto);  // Agregar un ítem al carrito
        Task<ResponseDto> RemoveItemFromCartAsync(string userId, int itemId);  // Eliminar un ítem del carrito
        Task<ResponseDto> ClearCartAsync(string userId);  // Vaciar el carrito
    }
}
