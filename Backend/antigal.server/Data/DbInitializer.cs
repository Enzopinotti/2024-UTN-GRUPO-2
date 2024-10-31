using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using antigal.server.Models; 
using antigal.server.Models.Dto;
using System.Linq;
public class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        // Crear los roles si no existen
        string[] roleNames = { "Admin", "User  " };
        string[] roleDescriptions = { "Administradores del sistema", "Usuarios regulares" };

        for (int i = 0; i < roleNames.Length; i++)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleNames[i]);
            if (!roleExist)
            {
                var role = new Role
                {
                    Name = roleNames[i],
                    Description = roleDescriptions[i] // Asignar descripción
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
            var adminUser = new User // Asegúrate de que User sea tu clase de usuario
            {
                UserName = "admin",
                Email = adminEmail,
                //FullName = "Administrador del Sistema",
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                // Confirmar el correo electrónico
                adminUser.EmailConfirmed = true; // Marcar el correo como confirmado
                await userManager.UpdateAsync(adminUser); // Actualizar el usuario en la base de datos

                await userManager.AddToRoleAsync(adminUser, "Admin"); // Asignar rol de administrador
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
