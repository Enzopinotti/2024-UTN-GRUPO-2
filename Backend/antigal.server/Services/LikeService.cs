using antigal.server.Data;
using antigal.server.Models;
using Microsoft.EntityFrameworkCore;

namespace antigal.server.Services
{
    public class LikeService
    {
        private readonly AppDbContext _context;

        public LikeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddLike(string userId, int productoId)
        {
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.ProductoId == productoId);

            if (existingLike != null)
            {
                return false; // Ya existe un like
            }
            var like = new Like { UserId = userId, ProductoId = productoId };
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveLike(string userId, int productoId)
        {
            var existingLike = await _context.Likes
                .FirstOrDefaultAsync(l => l.UserId == userId && l.ProductoId == productoId);

            if (existingLike == null)
                return false; // No existe el like

            _context.Likes.Remove(existingLike);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Producto>> GetUserLikes(string userId)
        {
            // Obtén todos los ProductoId que el usuario ha marcado con "like"
            var likedProductIds = await _context.Likes
                .Where(l => l.UserId == userId)
                .Select(l => l.ProductoId)
                .ToListAsync();

            // Consulta los productos correspondientes a esos ProductoId
            var likedProducts = await _context.Productos
                .Where(p => likedProductIds.Contains(p.idProducto))
                .ToListAsync();

            return likedProducts;
        }
    }
}
