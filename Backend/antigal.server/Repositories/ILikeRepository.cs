using antigal.server.Models;

namespace antigal.server.Repositories
{
    public interface ILikeRepository
    {
        Task<bool> AddLikeAsync(string userId, int productoId);
        Task<bool> RemoveLikeAsync(string userId, int productoId);
        Task<List<Producto>> GetUserLikesAsync(string userId);
    }
}
