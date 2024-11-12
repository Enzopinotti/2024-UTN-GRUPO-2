using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace antigal.server.Models
{
    public class Role : IdentityRole
    {
        public string? Description { get; set; }
    }
}
