using Microsoft.AspNetCore.Identity;
using antigal.server.Models;

namespace antigal.server.Data
{
    public class DbInitializer
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public DbInitializer(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task InitializeAsync()
        {
            await CreateRoles();
            await CreateAdminUser();
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

        private async Task CreateAdminUser()
        {
            var adminUser = await _userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                var user = new User
                {
                    UserName = "admin",
                    Email = "admin@admin.com",
                    EmailConfirmed = true // Confirma el email para simplificar pruebas
                };

                var result = await _userManager.CreateAsync(user, "Admin123#");
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin"); // Asigna el rol Admin al usuario
                }
                else
                {
                    // Manejar errores de creación del usuario
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                }
            }
        }
    }
}
