using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace antigal.server.Models
{
    public class User : IdentityUser
    {
        public string? FirstName{ get; set; }
        public string? LastName { get; set; }
        public string? ImagenUrl { get; set; }
    }
}
