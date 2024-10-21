﻿using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> Upload(IFormFile file, int? productoId = null, int? usuarioId = null, int? categoriaId = null)
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
    }
}
