using antigal.server.Data;
using antigal.server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace antigal.server.Services
{
    public class IdentityConfigurationService
    {
        public void ConfigureIdentity(IServiceCollection services)
        {
            services.AddIdentityApiEndpoints<User>(opt =>
            {
                // Configuraciones de Contraseña
                opt.Password.RequiredLength = 8;                        // Longitud mínima de la contraseña
                opt.Password.RequireDigit = true;                       // Requiere al menos un número
                opt.Password.RequireLowercase = true;                   // Requiere al menos una minúscula
                opt.Password.RequireUppercase = true;                   // Requiere al menos una mayúscula
                opt.Password.RequireNonAlphanumeric = false;            // Requiere caracteres especiales como símbolos
                opt.Password.RequiredUniqueChars = 1;                   // Requiere al menos un carácter único en la contraseña

                // Configuraciones de Usuario
                opt.User.RequireUniqueEmail = true;                     // El correo electrónico debe ser único para cada usuario
                opt.User.AllowedUserNameCharacters =                    // Caracteres permitidos para el nombre de usuario
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

                // Configuraciones de Bloqueo de Cuenta
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);  // Duración del bloqueo
                opt.Lockout.MaxFailedAccessAttempts = 5;                       // Intentos fallidos antes del bloqueo
                opt.Lockout.AllowedForNewUsers = true;                         // Permitir bloqueos de cuenta para nuevos usuarios

                // Configuraciones de Sesión e Inicio de Sesión
                opt.SignIn.RequireConfirmedEmail = true;               // Requiere que el correo esté confirmado antes de iniciar sesión
                opt.SignIn.RequireConfirmedPhoneNumber = false;        // Requiere que el teléfono esté confirmado antes de iniciar sesión
                opt.SignIn.RequireConfirmedAccount = false;            // Requiere confirmación completa de la cuenta antes de iniciar sesión

                // Configuraciones de Tokens
                opt.Tokens.AuthenticatorIssuer = "MyApp";               // Emisor de tokens de autenticación
                opt.Tokens.ChangeEmailTokenProvider = "Default";        // Proveedor de tokens para cambio de correo
                opt.Tokens.ChangePhoneNumberTokenProvider = "Default";  // Proveedor de tokens para cambio de teléfono
                opt.Tokens.EmailConfirmationTokenProvider = "Default";  // Proveedor de tokens para confirmación de correo
                opt.Tokens.PasswordResetTokenProvider = "Default";      // Proveedor de tokens para restablecimiento de contraseña
                opt.Tokens.AuthenticatorTokenProvider = "Default";      // Proveedor de tokens para autenticador de dos factores

                // Configuraciones de Cuenta
                opt.User.AllowedUserNameCharacters =                    // Define los caracteres permitidos para el nombre de usuario
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                opt.User.RequireUniqueEmail = true;                     // Requiere que el correo electrónico sea único por usuario

            })
            .AddDefaultUI()                                               // Añade la interfaz de usuario predeterminada para Identity
            .AddEntityFrameworkStores<AppDbContext>()                     // Usa Entity Framework para almacenar los datos de identidad
            .AddDefaultTokenProviders();                                  // Proveedores de token para restablecimiento de contraseña y autenticación de dos factores
        }
    }
}
