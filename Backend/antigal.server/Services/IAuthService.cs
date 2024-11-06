using System.Threading.Tasks;
using antigal.server.Models;
using antigal.server.Models.Dto;
using EmailService;
namespace antigal.server.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterUserAsync(UseForAuthenticationDto useForAuthentication);
        Task<(string AccessToken, string RefreshToken)> LoginUserAsync(LoginDto loginDto);
        Task<string> RefreshTokenAsync(string refreshToken);
        Task<User> GetUserByIdAsync(string userId);
        Task<bool> ConfirmEmailAsync(string userId, string token);
    }
}
