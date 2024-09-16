using antigal.server.Models;
using Microsoft.EntityFrameworkCore;

namespace antigal.server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Categoria> Categorias { get; set; }
    }
}
