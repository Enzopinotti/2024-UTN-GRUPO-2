using antigal.server.Models.Dto;
using antigal.server.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthAdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        public AuthAdminController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        /*
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                FullName = registerDto.FullName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                return Ok();
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login-admin")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, false, false);
            if (result.Succeeded)
            {
                return Ok();
            }

            return Unauthorized();
        }
        */

        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRoleAsync([FromBody] CreateRoleDto createRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = new Role { Name = createRoleDto.RoleName };
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }
    }
}
