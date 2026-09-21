using antigal.server.Models;
using antigal.server.Repositories;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace antigal.server.Services
{
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly IUnitOfWork _unitOfWork;

        public ImageService(Cloudinary cloudinary, IUnitOfWork unitOfWork)
        {
            _cloudinary = cloudinary;
            _unitOfWork = unitOfWork;
        }

        public async Task<Imagen> UploadImageAsync(IFormFile file, int? productoId = null, string? usuarioId = null, int? categoriaId = null)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file uploaded");
            }

            var uploadparams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "antigal-photos"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadparams);

            if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("Error uploading image");
            }

            var nuevaImagen = new Imagen
            {
                Url = uploadResult.SecureUrl.ToString(),
                PublicId = uploadResult.PublicId,
                ProductoId = productoId,
                UsuarioId = usuarioId,
                CategoriaId = categoriaId
            };

            return await _unitOfWork.Images.AddAsync(nuevaImagen);
        }

        public async Task<bool> DeleteImageAsync(int imageId)
        {
            var image = await _unitOfWork.Images.GetByIdAsync(imageId);
            if (image == null)
            {
                return false;
            }

            var deleteParams = new DeletionParams(image.PublicId);
            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

            if (deleteResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }

            await _unitOfWork.Images.DeleteAsync(image);
            return true;
        }

        public async Task<bool> DeleteImageByUrlAsync(string imageUrl)
        {
            var publicId = ExtractPublicIdFromUrl(imageUrl);
            if (string.IsNullOrEmpty(publicId))
            {
                throw new Exception("No se pudo extraer el PublicId de la URL proporcionada.");
            }

            var deleteParams = new DeletionParams(publicId);
            var deleteResult = await _cloudinary.DestroyAsync(deleteParams);

            if (deleteResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return false;
            }

            var image = await _unitOfWork.Images.GetByUrlAsync(imageUrl);
            if (image != null)
            {
                await _unitOfWork.Images.DeleteAsync(image);
            }

            return true;
        }

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
