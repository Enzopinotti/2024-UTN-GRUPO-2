using antigal.server.Models;
using antigal.server.Relationships;
using Microsoft.EntityFrameworkCore;


namespace antigal.server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }  // Esta línea es el constructor del contexto de la base de datos (AppDbContext).

        //MAPEO PRODUCTOS. Los DbSet se utilizan para agregar las clases que van a ser mapeadas a la base de datos.
        public DbSet <Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet <Imagen> Imagenes { get; set; }
        public DbSet <ProductoCategoria> ProductoCategoria { get; set; }

        //OnModelCreating se utiliza para establecer las asociaciones entre dos clases para que impacten en la base de datos desde .NET
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Producto>()
                .HasMany(p => p.imagenes)  //Un producto tiene muchas imagenes
                .WithOne(i => i.Producto)  //Una imagen pertenece a un producto
                .HasForeignKey(i => i.idProducto) //Establece la FK
                .OnDelete(DeleteBehavior.Cascade); //Establece la regla la cual dice que si se elimina un Producto, se elmininaran las imagenes de dicho producto.
       
            // Relacion muchos a muchos usando la tabla intermedia. ProductoCategoria
            modelBuilder.Entity<ProductoCategoria>()
                .HasKey(cp => new { cp.idProducto, cp.idCategoria });

            modelBuilder.Entity<ProductoCategoria>()
                .HasOne(cp => cp.Producto)
                .WithMany(p => p.CategoriaProductos)
                .HasForeignKey(cp => cp.idProducto);

            modelBuilder.Entity<ProductoCategoria>()
                .HasOne(cp => cp.Categoria)
                .WithMany(c => c.CategoriaProductos)
                .HasForeignKey(cp => cp.idCategoria);
        }

    }
}
