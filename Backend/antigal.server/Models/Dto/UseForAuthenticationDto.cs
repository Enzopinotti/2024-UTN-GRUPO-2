using System.ComponentModel.DataAnnotations;

namespace antigal.server.Models.Dto
{
    public class UseForAuthenticationDto
    {
        [Required(ErrorMessage = "Email es requerido")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Contraseña es requerido")]
        public string? Password { get; set; }
    }
}
