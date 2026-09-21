using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using antigal.server.Data;
using antigal.server.Models;

public class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        var bootstrapOptions = serviceProvider
            .GetRequiredService<IOptions<AdminBootstrapOptions>>()
            .Value;

        // Los roles forman parte del modelo de autorización y se crean si faltan.
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
                    NormalizedName = roleNames[i].ToUpperInvariant(),
                    Description = roleDescriptions[i]
                };

                var result = await roleManager.CreateAsync(role);
                if (result.Succeeded)
                {
                    Console.WriteLine($"Rol '{roleNames[i]}' creado con éxito.");
                }
                else
                {
                    Console.WriteLine(
                        $"Error al crear el rol '{roleNames[i]}': " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

        var bootstrap = AdminBootstrapPolicy.Resolve(
            bootstrapOptions,
            environment.IsDevelopment());

        if (bootstrap is null)
        {
            return;
        }

        var existingAdmin = await userManager.FindByEmailAsync(bootstrap.Email);
        if (existingAdmin is not null)
        {
            Console.WriteLine("El usuario administrador de bootstrap ya existe.");
            return;
        }

        var adminUser = new User
        {
            UserName = bootstrap.UserName,
            Email = bootstrap.Email,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(adminUser, bootstrap.Password);
        if (!createResult.Succeeded)
        {
            Console.WriteLine(
                "Error al crear el usuario administrador de bootstrap: " +
                string.Join(", ", createResult.Errors.Select(e => e.Description)));
            return;
        }

        var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
        if (!roleResult.Succeeded)
        {
            throw new InvalidOperationException(
                "El usuario administrador de bootstrap fue creado pero no pudo asignarse al rol Admin: " +
                string.Join(", ", roleResult.Errors.Select(e => e.Description)));
        }

        Console.WriteLine("Usuario administrador de bootstrap creado con éxito.");
    }
}
