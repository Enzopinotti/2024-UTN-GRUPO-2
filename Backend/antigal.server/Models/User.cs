using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace antigal.server.Models
{
    public class User : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
