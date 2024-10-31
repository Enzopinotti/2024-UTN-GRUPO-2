using Microsoft.AspNetCore.Mvc;
using antigal.server.Models.Dto;
using antigal.server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using antigal.server.JwtFeatures;
using CloudinaryDotNet.Actions;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly JwtHandler _jwtHandler;
        public RegisterController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration,
            IMapper mapper,
            JwtHandler jwtHandler)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _jwtHandler = jwtHandler;
        }

        // Registro de usuario regular (rol predeterminado: "User")
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto)
        {
            if (registerDto is null)
            {
                return BadRequest();
            }

            var user = _mapper.Map<User>(registerDto);

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new RegisterResponseDto { Errors = errors });
            }

            await _userManager.AddToRoleAsync(user, "User");

            return Ok(new { Message = "Usuario registrado exitosamente" });
        }

        /* Login de usuario
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "User")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
        */
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authtenticate([FromBody] UseForAuthenticationDto authenticationResponse)
        {
            var user = await _userManager.FindByNameAsync(authenticationResponse.Email!);
            if (user is null || !await _userManager.CheckPasswordAsync(user, authenticationResponse.Password!))
                return Unauthorized(new AuthenticationResponseDto { ErrorMessage = "Autenticacion invalida." });
            var token = _jwtHandler.CreateToken(user);
            return Ok(new AuthenticationResponseDto { IsAuthSuccesful = true, Token = token });
        }
    }
}
