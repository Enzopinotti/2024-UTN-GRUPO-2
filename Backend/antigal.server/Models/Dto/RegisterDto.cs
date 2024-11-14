using System.ComponentModel.DataAnnotations;

namespace antigal.server.Models.Dto
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "El nombre de usuario es requerido.")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "El correo electrónico es requerido.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "El nombre completo es requerido.")]
        public required string FullName { get; set; }
    }
}
