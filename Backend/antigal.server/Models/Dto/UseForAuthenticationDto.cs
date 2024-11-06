using System.ComponentModel.DataAnnotations;

namespace antigal.server.Models.Dto
{
    public class UseForAuthenticationDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email es requerido")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Contraseña es requerido")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]

        public string? ConfirmPassword { get; set; }

        public string? ClientUri { get; set; }
    }
}