using antigal.server.Models;
using antigal.server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : ControllerBase
    {
        private readonly LikeService _likeService;
        private readonly UserManager<User> _userManager;

        public LikesController(LikeService likeService, UserManager<User> userManager)
        {
            _likeService = likeService;
            _userManager = userManager;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddLike(int productoId)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
                return Unauthorized();

            var result = await _likeService.AddLike(userId, productoId);
            return result ? Ok() : BadRequest("Agregado a favoritos!");
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveLike(int productoId)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
                return Unauthorized();

            var result = await _likeService.RemoveLike(userId, productoId);
            return result ? Ok() : NotFound("Eliminado de favoritos!");
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetUserLikes()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
                return Unauthorized();

            var likes = await _likeService.GetUserLikes(userId);
            return Ok(likes);
        }
    }
}
