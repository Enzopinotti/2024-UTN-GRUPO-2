﻿using System.Threading.Tasks;
using antigal.server.Models;
using antigal.server.Models.Dto;

namespace antigal.server.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterUserAsync(RegisterDto registerDto);
        Task<UserDto> ValidateUserAsync(string token);
        Task<string> LoginUserAsync(LoginDto loginDto);
    }
}
