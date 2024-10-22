using antigal.server.Data;
using antigal.server.Models;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public interface IImageService
    {
        Task<Imagen> UploadImageAsync(IFormFile file, int? productoId = null, string? usuarioId = null, int? categoriaId = null);
        Task<bool> DeleteImageAsync(int imageId);
        Task<bool> DeleteImageByUrlAsync(string imageUrl);
    }
}
