using antigal.server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace antigal.server.SeedConfiguration
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(
                new Role
                {
                    Id = "639de93f-7876-4fff-96ec-37f8bd3bf180", // GUID fijo para el rol Visitor
                    Name = "Visitor",
                    NormalizedName = "VISITOR",
                    Description = "The visitor role for the user"
                },
                new Role
                {
                    Id = "d8a9f8f8-3d65-4b2a-9b2f-3a1c4b2c1234", // GUID fijo para el rol User
                    Name = "User",
                    NormalizedName = "USER",
                    Description = "The regular user role"
                },
                new Role
                {
                    Id = "a9b5f83e-92c3-4c5e-94de-4d6a6e4f82a9", // Nuevo GUID generado para el rol Admin
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Description = "The admin role for the user"
                }
            );
        }
    }
}
