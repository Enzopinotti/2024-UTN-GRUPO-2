using antigal.server.Models;
using antigal.server.Repositories;

namespace antigal.server.Services
{
    public class LikeService : ILikeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LikeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<bool> AddLike(string userId, int productoId)
        {
            return _unitOfWork.Likes.AddLikeAsync(userId, productoId);
        }

        public Task<bool> RemoveLike(string userId, int productoId)
        {
            return _unitOfWork.Likes.RemoveLikeAsync(userId, productoId);
        }

        public Task<List<Producto>> GetUserLikes(string userId)
        {
            return _unitOfWork.Likes.GetUserLikesAsync(userId);
        }
    }
}
