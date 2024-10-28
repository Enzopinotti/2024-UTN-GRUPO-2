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

            // Configurar Identity
            builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Configuración de JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
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
            // Inyección del servicio ICartService y su implementación CartService
            builder.Services.AddScoped<ICartService, CartService>();

            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ServiceToken>();

            //*********** REPOSITORIES ***********//

            // Inyección del repositorio IProductCategoryRepository y su implementación ProductCategoryRepository
            builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
            // Inyección del repositorio ICategoriaRepository y su implementación CategoriaRepository
            builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            // Inyección del repositorio ICartRepository y su implementación CartRepository
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            // Inyección del servicio IOrderService y su implementación OrderService
            builder.Services.AddScoped<IOrderService, OrderService>();
            // Inyección del repositorio IOrderRepository y su implementación OrderRepository
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            // Agregar servicios de CORS (sirve para restringir metodos, origen de solicitudes, etc) SEGURIDAD
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost",
                    builder => builder.WithOrigins("http://localhost:3000") // Permitir acceso desde el frontend
                                      .AllowAnyMethod() // Permitir todos los métodos HTTP (GET, POST, etc.)
                                      .AllowAnyHeader()); // Permitir todas las cabeceras
            });

            // Controllers
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });

            // Validaciones
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();
            builder.Services.AddFluentValidationAutoValidation();

            // Agregar otros servicios como Swagger si es necesario
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    await DbInitializer.Initialize(services);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al inicializar la base de datos: {ex.Message}");
                }
            }

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

            app.Run();
        }
        
    }

}
