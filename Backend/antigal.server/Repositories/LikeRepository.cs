using antigal.server.Data;
using antigal.server.Models;
using Microsoft.EntityFrameworkCore;

namespace antigal.server.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly AppDbContext _context;

        public LikeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddLikeAsync(string userId, int productoId)
        {
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.ProductoId == productoId);

            if (existingLike != null)
            {
                return false;
            }

            var like = new Like { UserId = userId, ProductoId = productoId };
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveLikeAsync(string userId, int productoId)
        {
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.ProductoId == productoId);

            if (existingLike == null)
            {
                return false;
            }

            _context.Likes.Remove(existingLike);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Producto>> GetUserLikesAsync(string userId)
        {
            var likedProductIds = await _context.Likes
                .Where(l => l.UserId == userId)
                .Select(l => l.ProductoId)
                .ToListAsync();

            return await _context.Productos
                .Where(p => likedProductIds.Contains(p.idProducto))
                .ToListAsync();
        }
    }
}
