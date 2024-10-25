using antigal.server.Data;
using antigal.server.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

            var uploadparams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "antigal-photos"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadparams);

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

                // Actualizar la entidad correspondiente con la URL
                if (productoId.HasValue)
                {
                    var producto = await _context.Productos.FindAsync(productoId.Value);
                    if (producto != null)
                    {
                        producto.ImagenUrls.Add(nuevaImagen.Url); // Agregar la URL a la lista de URLs del producto
                        await _context.SaveChangesAsync();
                    }
                }
                else if (!string.IsNullOrEmpty(usuarioId))
                {
                    var usuario = await _context.Users.FindAsync(usuarioId);
                    if (usuario != null)
                    {
                        usuario.ImagenUrl = nuevaImagen.Url; // Asignar la URL directamente
                        await _context.SaveChangesAsync();
                    }
                }
                else if (categoriaId.HasValue)
                {
                    var categoria = await _context.Categorias.FindAsync(categoriaId.Value);
                    if (categoria != null)
                    {
                        categoria.ImagenUrl = nuevaImagen.Url; // Asignar la URL directamente
                        await _context.SaveChangesAsync();
                    }
                }

                return nuevaImagen;
            }
            else
            {
                throw new Exception("Error uploading image");
            }

        }
    }
}
