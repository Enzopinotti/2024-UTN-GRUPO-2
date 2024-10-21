using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using antigal.server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using antigal.server.Models;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly UserManager<IdentityUser> _userManager; // UserManager para acceder al usuario

        public ImageController(IImageService imageService, UserManager<IdentityUser> userManager)
        {
            _imageService = imageService;
            _userManager = userManager;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file, int? productoId = null, string? usuarioId = null, int? categoriaId = null)
        {
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

        [HttpPost("change-profile-image")]
        //[Authorize] // Solo usuarios autenticados pueden cambiar su imagen de perfil
        public async Task<IActionResult> ChangeProfileImage(IFormFile file)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var imagen = await _imageService.UploadImageAsync(file, null, userId, null);

                return Ok(new { id = imagen.Id, url = imagen.Url });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
