using antigal.server.Models;
using Microsoft.AspNetCore.Identity;

namespace antigal.server.Services
{
    public interface ITokenService
    {
        Task<string> GenerateJwtToken(UserManager<User> userManager, User user);
        Task<string> GenerateEmailConfirmationTokenAsync(User user);
    }
}
