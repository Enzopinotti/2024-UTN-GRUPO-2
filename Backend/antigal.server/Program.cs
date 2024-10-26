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
using System.Security.Claims;
using NPOI.SS.Formula.Functions;
using NPOI.HPSF;
using Humanizer;
using NuGet.Common;
using antigal.server.Models.Dto;
using IEmailSender = antigal.server.Services.IEmailSender;

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

            builder.Services.AddTransient<DbInitializer>();
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

            // Extender los endpoints
            app.MapPost("/register-antigal", async(UserManager<User> userManager, IEmailSender emailSender, RegisterDto registerDto) =>
            {
                var user = new User { UserName = registerDto.Email, Email = registerDto.Email };
                var result = await userManager.CreateAsync(user, registerDto.Password);
                if (result.Succeeded)
                {
                    // Generar el token de confirmación
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = $"https://localhost:5000/confirm-email?userId={user.Id}&token={token}";

                    // Enviar el correo de confirmación
                    await emailSender.SendEmailAsync(user.Email, "Confirm your email",
                        $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>link</a>");

                    return Results.Ok("Registration successful. Please check your email to confirm.");
                }

                return Results.BadRequest(result.Errors);
            });

            app.MapGet("/confirmar-email-antigal", async(UserManager<User> userManager, string userId, string token) =>
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null) return Results.NotFound();

                var result = await userManager.ConfirmEmailAsync(user, token);
                return result.Succeeded ? Results.Ok("Email confirmed successfully.") : Results.BadRequest("Email confirmation failed.");
            });

            app.MapPost("/logout-antigal", async (SignInManager<User> signInManager) =>
            {
                await signInManager.SignOutAsync().ConfigureAwait(false);
            }).RequireAuthorization(); // mata el token

            app.MapGet("/pingauth-antigal", (ClaimsPrincipal user) =>
            {
                var email = user.FindFirstValue(ClaimTypes.Email);
                return Results.Json(new { Email = email }); ;
            }).RequireAuthorization();  //comprueba si esta autentificado

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Llamar al inicializador de base de datos
            await InitializeDatabase(app.Services);

            app.Run();
        }

        private static async Task InitializeDatabase(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
            await dbInitializer.InitializeAsync();
        }

    }
}
