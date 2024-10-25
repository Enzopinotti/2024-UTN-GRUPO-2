using Microsoft.EntityFrameworkCore;
using antigal.server.Data;
using antigal.server.Services;
using antigal.server.Repositories;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Microsoft.AspNetCore.Identity;
using antigal.server.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MathNet.Numerics.Interpolation;
using CloudinaryDotNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace antigal.server
{
    public class Program
    {
        public static async Task Main(string[] args) // Cambiar a Task
        {
            var builder = WebApplication.CreateBuilder(args);

            // Agregar el contexto de la base de datos al contenedor de servicios
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            

            // Crear instancia de IdentityConfigurationService
            var identityConfigService = new IdentityConfigurationService();
            identityConfigService.ConfigureIdentity(builder.Services);

            builder.Services.ConfigureApplicationCookie(options => 
                                { options.Cookie.SameSite = SameSiteMode.None; });
            
            builder.Services.AddAuthorization();

            //Cloudinary para imagenes
            var cloudinaryConfig = builder.Configuration.GetSection("Cloudinary");

            var cloudinary = new Cloudinary(new Account(
                cloudinaryConfig["CloudName"],
                cloudinaryConfig["ApiKey"],
                cloudinaryConfig["ApiSecret"]
                ));

            builder.Services.AddSingleton(cloudinary);

            builder.Services.AddScoped<IImageService, ImageService>();


            //*********** SERVICES ***********//

            // Inyección del servicio IProductService y su implementación ProductService
            builder.Services.AddScoped<IProductService, ProductService>();
            // Inyección del servicio ICategoryService y su implementación CategoryService
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            // Inyección del servicio IProductCategoryService y su implementación ProductCategoryService
            builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();
            // Inyección del servicio ICartService y su implementación CartService
            builder.Services.AddScoped<ICartService, CartService>();

            //*********** REPOSITORIES ***********//

            // Inyección del repositorio IProductCategoryRepository y su implementación ProductCategoryRepository
            builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            // Inyección del repositorio ICategoriaRepository y su implementación CategoriaRepository
            builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            // Inyección del repositorio ICartRepository y su implementación CartRepository
            builder.Services.AddScoped<ICartRepository, CartRepository>();

            // Agregar servicios de CORS (sirve para restringir metodos, origen de solicitudes, etc) SEGURIDAD
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost",
                    builder => builder.WithOrigins("http://localhost:3000") // Permitir acceso desde el frontend
                                      .AllowAnyMethod() // Permitir todos los métodos HTTP (GET, POST, etc.)
                                      .AllowAnyHeader()); // Permitir todas las cabeceras
            });

            // Controllers
            builder.Services.AddControllers();

            // Validaciones
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();
            builder.Services.AddFluentValidationAutoValidation();

            // Agregar otros servicios como Swagger si es necesario
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Usar la política de CORS
            app.UseCors("AllowLocalhost");

            // Configurar el middleware de la aplicación
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapGroup("/identity").MapIdentityApi<User>();

            app.MapPost("/logout", async (SignInManager<User> signInManager) =>
            {
                await signInManager.SignOutAsync().ConfigureAwait(false);
            }).RequireAuthorization(); // So that only authorized users can use this endpoint

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            string[] roleNames = { "Admin", "Usuario" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new Role { Name = roleName });
                }
            }
        }


    }
}
