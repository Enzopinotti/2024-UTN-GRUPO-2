// Controllers/LikesController.cs
using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace antigal.server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly ILikeService _likeService;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<LikesController> _logger;

        public LikesController(ILikeService likeService, UserManager<User> userManager, ILogger<LikesController> logger)
        {
            _likeService = likeService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddLike(int productoId)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
                return Unauthorized(new { message = "Usuario no autenticado. Por favor, inicie sesión." });

            var result = await _likeService.AddLike(userId, productoId);
            if (result)
            {
                return Ok(new { message = "Producto agregado a favoritos." });
            }
            else
            {
                return BadRequest(new { message = "El producto ya está en tus favoritos." });
            }
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveLike(int productoId)
        {
            var userId = _userManager.GetUserId(User);

            _logger.LogInformation("UserId retrieved: {UserId}", userId);

            if (userId == null)
            {
                _logger.LogWarning("Intento de eliminar like sin autenticación.");
                return Unauthorized(new { message = "Usuario no autenticado. Por favor, inicie sesión." });
            }

            var result = await _likeService.RemoveLike(userId, productoId);
            if (result)
            {
                _logger.LogInformation("Producto {ProductoId} eliminado de favoritos por el usuario {UserId}.", productoId, userId);
                return Ok(new { message = "Producto eliminado de favoritos." });
            }
            else
            {
                _logger.LogInformation("El producto {ProductoId} no estaba en favoritos del usuario {UserId}.", productoId, userId);
                return NotFound(new { message = "El producto no estaba en tus favoritos." });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetUserLikes()
        {
            var userId = _userManager.GetUserId(User);

            _logger.LogInformation("UserId retrieved: {UserId}", userId);

            if (userId == null)
            {
                _logger.LogWarning("Intento de obtener likes sin autenticación.");
                return Unauthorized(new { message = "Usuario no autenticado. Por favor, inicie sesión." });
            }

            var likes = await _likeService.GetUserLikes(userId);
            _logger.LogInformation("Likes obtenidos para el usuario {UserId}.", userId);
            return Ok(new { likes });
        }
    }
}