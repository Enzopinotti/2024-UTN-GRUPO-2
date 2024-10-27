using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using antigal.server.Models.Dto;
using antigal.server.Models;
using antigal.server.Services;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IEmailSender _emailSender;
    private readonly ICartService _cartService;
    private readonly ILogger<AuthController> _logger;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender, ICartService cartService, ILogger<AuthController> logger, ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
        _cartService = cartService;
        _logger = logger;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var user = new User { UserName = registerDto.Email, Email = registerDto.Email };
        var result = await _userManager.CreateAsync(user, registerDto.Password);
        if (result.Succeeded)
        {
            // Generar el token de confirmación
            var token = await _tokenService.GenerateEmailConfirmationTokenAsync(user); // Usar TokenService
            var confirmationLink = $"https://localhost:7255/api/auth/confirmar-email?userId={user.Id}&token={token}";

            // Enviar el correo de confirmación
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>link</a>");

            // Crear un carrito vacío para el nuevo usuario
            var cartResponse = _cartService.CreateCart(user.Id);
            if (!cartResponse.IsSuccess)
            {
                // Registrar un mensaje de error si la creación del carrito falla
                _logger.LogError($"No se pudo crear el carrito para el usuario con ID: {user.Id}. Error: {cartResponse.Message}");
            }

            return Ok("Registration successful. Please check your email to confirm.");
        }

        return BadRequest(result.Errors);
    }

    [HttpGet("confirmar-email")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            user.EmailConfirmed = true; 
            await _userManager.UpdateAsync(user);
            return Ok("Email confirmed successfully.");
        }
        return BadRequest("Email confirmation failed.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Email);
            var tokenService = HttpContext.RequestServices.GetRequiredService<TokenService>();
            var token = await tokenService.GenerateJwtToken(_userManager, user);

            if (loginDto.RememberMe)
            {
                // Si el usuario desea ser recordado, establece una cookie persistente
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddDays(30) // Configura la duración de la cookie
                };
                Response.Cookies.Append("CookieName", token, cookieOptions);
            }

            return Ok(new { Token = token, Message = "Login successful." });
        }

        return Unauthorized(new { Message = "Invalid login attempt." });
    }



    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok("Logged out successfully.");
    }

    [HttpGet("pingauth")]
    [Authorize]
    public IActionResult PingAuth()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        return Ok(new { Email = email });
    }
}
