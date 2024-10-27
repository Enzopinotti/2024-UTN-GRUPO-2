using antigal.server.Data;
using antigal.server.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace antigal.server.Services
{
    public class IdentityConfigurationService
    {
       
        public void ConfigureIdentity(IServiceCollection services)
        {

            services.AddTransient<IEmailSender, EmailSender>();

           
            services.AddIdentity<User, Role>(options =>
            {
                // Configuraciones de contraseña
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;

                // Configuraciones de usuario
                options.User.RequireUniqueEmail = true;

                // Configuraciones de bloqueo
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // Configuraciones de sesión
                options.SignIn.RequireConfirmedEmail = true;
            })
                   .AddEntityFrameworkStores<AppDbContext>()
                   .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.LoginPath = "/Auth/Login"; // Define tu ruta de inicio de sesión personalizada
                options.LogoutPath = "/Auth/Logout"; // Define tu ruta de cierre de sesión personalizada
                options.AccessDeniedPath = "/Auth/AccessDenied"; // Define tu ruta de acceso denegado personalizada
            });
        }
    }
}
