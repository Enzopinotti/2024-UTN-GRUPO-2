using Microsoft.AspNetCore.Identity;
using antigal.server.Models;

namespace antigal.server.Data
{
    public class DbInitializer
    {
        private readonly RoleManager<Role> _roleManager;

        public DbInitializer(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task InitializeAsync()
        {
            await CreateRoles();
        }

        private async Task CreateRoles()
        {
            string[] roleNames = { "Admin", "User " };
            foreach (var roleName in roleNames)
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new Role { Name = roleName });
                }
            }
        }
    }
}
