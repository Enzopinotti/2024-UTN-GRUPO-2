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
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;


namespace antigal.server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Agregar el contexto de la base de datos al contenedor de servicios
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configurar Identity
            builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            //Configuración de HttpClient
            builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                client.BaseAddress = new Uri("https://your-auth0-domain/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_API_TOKEN"); // Si es necesario
            });

            // Configurar la autenticación JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/"; // Reemplaza con tu dominio de Auth0
                options.Audience = builder.Configuration["Auth0:Audience"]; // Reemplaza con el Identifier de tu API en Auth0

                // Configurar validaciones adicionales si es necesario
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://{builder.Configuration["Auth0:Domain"]}/",
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Auth0:Audience"],
                    ValidateLifetime = true,
                };
            });


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

            // Registra el AuthService
            builder.Services.AddScoped<IAuthService, AuthService>();

            //*********** REPOSITORIES ***********//

            // Inyección del repositorio IProductCategoryRepository y su implementación ProductCategoryRepository
            builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            // Inyección del repositorio ICategoriaRepository y su implementación CategoriaRepository
            builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

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

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Crear roles
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                CreateRoles(serviceProvider).Wait();
            }


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
