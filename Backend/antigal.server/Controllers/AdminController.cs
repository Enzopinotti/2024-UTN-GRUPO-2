/*using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using antigal.server.Models;
using antigal.server.Models.Dto;

namespace antigal.server.Controllers
{
    [Authorize(Roles = "Admin")]  // Solo accesible para usuarios con el rol Admin
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;  // Inyectamos RoleManager para gestionar roles

        public AdminController(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: api/Admin/Users
        [HttpGet("Users")]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users
                .Select(user => new
                {
                    user.FullName,
                    user.Email,
                    user.ImagenUrl // Asegúrate de tener esta propiedad en tu modelo User
                })
                .ToList();

            return Ok(users);
        }

        // POST: api/Admin/CreateRole
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRoleAsync([FromBody] CreateRoleDto createRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var roleExists = await _roleManager.RoleExistsAsync(createRoleDto.RoleName);
            if (roleExists)
            {
                return BadRequest(new { Message = $"El rol {createRoleDto.RoleName} ya existe." });
            }

            var role = new Role { Name = createRoleDto.RoleName };
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return Ok(new { Message = $"Rol {createRoleDto.RoleName} creado exitosamente." });
            }

            return BadRequest(result.Errors);
        }
    }
}
*/