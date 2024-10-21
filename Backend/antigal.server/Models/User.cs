using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace antigal.server.Models
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; }
        // Relación uno a uno con Imagen
        public virtual Imagen? Imagen { get; set; } // Solo puede tener una imagen
    }
}
