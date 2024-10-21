using antigal.server.Data;
using antigal.server.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace antigal.server.Services
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly AppDbContext _context;

        public ImageService(Cloudinary cloudinary, AppDbContext context)
        {
            _cloudinary = cloudinary;
            _context = context;
        }

        public async Task<Imagen> UploadImageAsync(IFormFile file, int? productoId = null, string? usuarioId = null, int? categoriaId = null)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file uploaded");
            }

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "antigal-photos" // Directorio en tu cuenta de Cloudinary
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var nuevaImagen = new Imagen
                {
                    Url = uploadResult.SecureUrl.ToString(),
                    ProductoId = productoId,
                    UsuarioId = usuarioId,
                    CategoriaId = categoriaId
                };

                _context.Imagenes.Add(nuevaImagen);
                await _context.SaveChangesAsync();

                return nuevaImagen;
            }
            else
            {
                throw new Exception("Error uploading image");
            }
        }
    }
}
