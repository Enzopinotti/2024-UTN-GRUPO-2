using Microsoft.EntityFrameworkCore;
using antigal.server.Data;
using antigal.server.Services;
using antigal.server.Repositories;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Microsoft.AspNetCore.Identity;
using antigal.server.Models;

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

            //*********** SERVICES ***********//

            // Inyección del servicio IProductService y su implementación ProductService
            builder.Services.AddScoped<IProductService, ProductService>();
            // Inyección del servicio ICategoryService y su implementación CategoryService
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            // Inyección del servicio IProductCategoryService y su implementación ProductCategoryService
            builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();

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

            app.Run();
        }
    }
}
