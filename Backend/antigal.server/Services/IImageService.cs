using antigal.server.Models;
using Microsoft.AspNetCore.Http;

namespace antigal.server.Services
{
    public interface IImageService
    {
        Task<Imagen> UploadImageAsync(IFormFile file, int? productoId = null, string? usuarioId = null, int? categoriaId = null);
        Task<bool> DeleteImageAsync(int imageId);
        Task<bool> DeleteImageByUrlAsync(string imageUrl);
    }
}
