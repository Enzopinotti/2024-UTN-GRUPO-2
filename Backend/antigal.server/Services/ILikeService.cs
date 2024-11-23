// Services/ILikeService.cs
using antigal.server.Models;

namespace antigal.server.Services
{
    public interface ILikeService
    {
        Task<bool> AddLike(string userId, int productoId);
        Task<bool> RemoveLike(string userId, int productoId);
        Task<List<Producto>> GetUserLikes(string userId);
    }
}
