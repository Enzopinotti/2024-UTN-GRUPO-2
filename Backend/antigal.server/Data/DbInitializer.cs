using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using antigal.server.Models;
using System.Linq;

public class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        // Crear los roles si no existen
        string[] roleNames = { "Admin", "User", "Visitor" };
        string[] roleDescriptions = { "Administradores del sistema", "Usuarios regulares", "Visitantes" };

        for (int i = 0; i < roleNames.Length; i++)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleNames[i]);
            if (!roleExist)
            {
                var role = new Role
                {
                    Name = roleNames[i],
                    NormalizedName = roleNames[i].ToUpper(),
                    Description = roleDescriptions[i]
                };

                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    Console.WriteLine($"Rol '{roleNames[i]}' creado con éxito.");
                }
                else
                {
                    Console.WriteLine($"Error al crear el rol '{roleNames[i]}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }

        // Crear el usuario administrador si no existe
        string adminEmail = "admin@gmail.com";
        string adminPassword = "Admin123!";

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin == null)
        {
            var adminUser = new User
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true // Confirmar el email directamente
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"Usuario administrador '{adminUser.UserName}' creado con éxito.");
            }
            else
            {
                Console.WriteLine($"Error al crear el usuario administrador: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            Console.WriteLine($"El usuario administrador '{adminEmail}' ya existe.");
        }
    }
}
