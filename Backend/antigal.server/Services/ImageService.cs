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
                    PublicId = uploadResult.PublicId, 
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

        public async Task<bool> DeleteImageAsync(int imageId)
        {
            var image = await _context.Imagenes.FindAsync(imageId);
            if (image == null)
            {
                return false; // La imagen no existe
            }

            // Eliminar la imagen de Cloudinary
            var deleteParams = new DeletionParams(image.PublicId);
            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

            if (deleteResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _context.Imagenes.Remove(image);

                // Actualizar el producto, usuario o categoría según corresponda
                if (image.ProductoId.HasValue)
                {
                    var producto = await _context.Productos.FindAsync(image.ProductoId.Value);
                    if (producto != null)
                    {
                        producto.ImagenUrls.Remove(image.Url); 
                    }
                }
                else if (!string.IsNullOrEmpty(image.UsuarioId))
                {
                    var usuario = await _context.Users.FindAsync(image.UsuarioId);
                    if (usuario != null)
                    {
                        usuario.ImagenUrl = null;
                    }
                }
                else if (image.CategoriaId.HasValue)
                {
                    var categoria = await _context.Categorias.FindAsync(image.CategoriaId.Value);
                    if (categoria != null)
                    {
                        categoria.ImagenUrl = null; 
                    }
                }

                await _context.SaveChangesAsync(); 
                return true; // Eliminación exitosa
            }
            else
            {
                return false; // Error al eliminar la imagen
            }
        }

        public async Task<bool> DeleteImageByUrlAsync(string imageUrl)
        {
            var publicId = ExtractPublicIdFromUrl(imageUrl);
            if (string.IsNullOrEmpty(publicId))
            {
                throw new Exception("No se pudo extraer el PublicId de la URL proporcionada.");
            }

            // Eliminar la imagen de Cloudinary
            var deleteParams = new DeletionParams(publicId);
            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

            if (deleteResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var image = await _context.Imagenes.FirstOrDefaultAsync(i => i.Url == imageUrl);
                if (image != null)
                {
                    _context.Imagenes.Remove(image);

                    // Actualizar el producto, usuario o categoría según corresponda
                    if (image.ProductoId.HasValue)
                    {
                        var producto = await _context.Productos.FindAsync(image.ProductoId.Value);
                        if (producto != null)
                        {
                            producto.ImagenUrls.Remove(image.Url); 
                        }
                    }
                    else if (!string.IsNullOrEmpty(image.UsuarioId))
                    {
                        var usuario = await _context.Users.FindAsync(image.UsuarioId);
                        if (usuario != null)
                        {
                            usuario.ImagenUrl = null; 
                        }
                    }
                    else if (image.CategoriaId.HasValue)
                    {
                        var categoria = await _context.Categorias.FindAsync(image.CategoriaId.Value);
                        if (categoria != null)
                        {
                            categoria.ImagenUrl = null; 
                        }
                    }

                    await _context.SaveChangesAsync();
                }
                return true; // Eliminación exitosa
            }
            else
            {
                return false; // Error al eliminar la imagen
            }
        }

        //metodo para extraer la PublicId desde la url
        private string ExtractPublicIdFromUrl(string imageUrl)
        {
            var uri = new Uri(imageUrl);
            var segments = uri.Segments;

            if (segments.Length > 0)
            {
                var publicIdWithExtension = segments[segments.Length - 1];
                var publicId = publicIdWithExtension.Split('.')[0];
                return publicId;
            }

            throw new InvalidOperationException("No se pudo extraer el Public ID de la URL proporcionada.");
        }

    }
}
