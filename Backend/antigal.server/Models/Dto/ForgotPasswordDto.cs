using System.ComponentModel.DataAnnotations;

namespace antigal.server.Models.Dto
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        required public string Email { get; set; }    
        [Required]
        required public string ClientUri { get; set; }
    }
}
