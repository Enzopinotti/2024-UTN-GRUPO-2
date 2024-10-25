using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public IActionResult GetUsers()
        {
            // Lógica para obtener usuarios
            return Ok(new { message = "Usuarios obtenidos correctamente." });
        }
    }
}
