using antigal.server.Models;
using Microsoft.EntityFrameworkCore;
using antigal.server.Models;

namespace antigal.server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        //MAPEO PRODUCTOS
        public DbSet <Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
    }
}
