using antigal.server.Data;
using antigal.server.Models;
using Microsoft.EntityFrameworkCore;

namespace antigal.server.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly AppDbContext _context;

        public ImageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Imagen> AddAsync(Imagen image)
        {
            _context.Imagenes.Add(image);
            await ApplyAssociationAsync(image, add: true);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<Imagen?> GetByIdAsync(int imageId)
        {
            return await _context.Imagenes.FindAsync(imageId);
        }

        public Task<Imagen?> GetByUrlAsync(string imageUrl)
        {
            return _context.Imagenes.FirstOrDefaultAsync(image => image.Url == imageUrl);
        }

        public async Task DeleteAsync(Imagen image)
        {
            _context.Imagenes.Remove(image);
            await ApplyAssociationAsync(image, add: false);
            await _context.SaveChangesAsync();
        }

        private async Task ApplyAssociationAsync(Imagen image, bool add)
        {
            if (image.ProductoId.HasValue)
            {
                var producto = await _context.Productos.FindAsync(image.ProductoId.Value);
                if (producto is not null)
                {
                    if (add)
                    {
                        producto.ImagenUrls.Add(image.Url);
                    }
                    else
                    {
                        producto.ImagenUrls.Remove(image.Url);
                    }
                }

                return;
            }

            if (!string.IsNullOrEmpty(image.UsuarioId))
            {
                var usuario = await _context.Users.FindAsync(image.UsuarioId);
                if (usuario is not null)
                {
                    usuario.ImagenUrl = add ? image.Url : null;
                }

                return;
            }

            if (image.CategoriaId.HasValue)
            {
                var categoria = await _context.Categorias.FindAsync(image.CategoriaId.Value);
                if (categoria is not null)
                {
                    categoria.ImagenUrl = add ? image.Url : null;
                }
            }
        }
    }
}
