using antigal.server.Models;
using antigal.server.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public RoleController(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // Crear un nuevo rol
        [HttpPost("create")]
        [Authorize(Roles = "Admin")] // Solo un usuario con rol Admin puede crear roles
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                return BadRequest("Role already exists.");
            }

            var role = new Role { Name = roleName };
            await _roleManager.CreateAsync(role);
            return Ok("Role created successfully.");
        }

        // Asignar un rol a un usuario
        [HttpPost("assign")]
        [Authorize(Roles = "Admin")] // Solo un usuario con rol Admin puede asignar roles
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound("User  not found.");

            var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
            if (!roleExists) return BadRequest("Role does not exist.");

            await _userManager.AddToRoleAsync(user, model.RoleName);
            return Ok("Role assigned successfully.");
        }
    }
}
