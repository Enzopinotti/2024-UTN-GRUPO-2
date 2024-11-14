using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using antigal.server.Services;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, int? productoId = null, string? usuarioId = null, int? categoriaId = null)
        {
            Console.WriteLine("llega: " + file + productoId);
            try
            {
                var imagen = await _imageService.UploadImageAsync(file, productoId, usuarioId, categoriaId);
                return Ok(new { id = imagen.Id, url = imagen.Url });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            try
            {
                var result = await _imageService.DeleteImageAsync(imageId);
                if (result)
                {
                    return Ok("Imagen eliminada con éxito");
                }
                else
                {
                    return NotFound("La imagen no existe");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("eliminar-por-url")]
        public async Task<IActionResult> DeleteImageByUrl([FromQuery] string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return BadRequest("La URL de la imagen no puede estar vacía.");
            }

            try
            {
                var result = await _imageService.DeleteImageByUrlAsync(imageUrl);
                if (result)
                {
                    return Ok("Imagen eliminada con éxito.");
                }
                else
                {
                    return NotFound("No se pudo eliminar la imagen. Puede que no exista.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
