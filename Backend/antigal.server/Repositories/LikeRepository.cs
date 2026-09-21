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
            var like = new Like { UserId = userId, ProductoId = productoId };
            _context.Likes.Add(like);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                // A concurrent request may have inserted the same favorite first.
                // Detach this failed insert before verifying whether that exact pair now exists.
                _context.Entry(like).State = EntityState.Detached;

                var duplicateExists = await _context.Likes
                    .AsNoTracking()
                    .AnyAsync(l => l.UserId == userId && l.ProductoId == productoId);

                if (duplicateExists)
                {
                    return false;
                }

                throw;
            }
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
