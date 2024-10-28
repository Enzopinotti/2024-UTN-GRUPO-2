using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using antigal.server.Models;

namespace antigal.server.Controllers
{
    [Authorize(Roles = "Admin")]  // Solo accesible para el rol Admin
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public AdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // GET: api/Admin/Users
        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = _userManager.Users
                .Select(user => new
                {
                    user.FullName,
                    user.Email,
                    user.ImagenUrl // Asegúrate de tener esta propiedad en tu modelo User
                }).ToList();

            return Ok(users);
        }
    }
}
