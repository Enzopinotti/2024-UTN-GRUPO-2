using antigal.server.Models;
using antigal.server.Relationships;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace antigal.server.Data
{
    public class AppDbContext : IdentityDbContext<User, Role, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }  // Esta l�nea es el constructor del contexto de la base de datos (AppDbContext).

        //MAPEO PRODUCTOS. Los DbSet se utilizan para agregar las clases que van a ser mapeadas a la base de datos.
        public DbSet <Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet <Imagen> Imagenes { get; set; }
        public DbSet <ProductoCategoria> ProductoCategoria { get; set; }

        //OnModelCreating se utiliza para establecer las asociaciones entre dos clases para que impacten en la base de datos desde .NET
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Llama al m�todo base
            
            modelBuilder.Entity<Imagen>()
                .HasOne(i => i.Producto)
                .WithMany(p => p.Imagenes) // Aseg�rate de que Producto tenga una lista de im�genes
                .HasForeignKey(i => i.ProductoId)
                .OnDelete(DeleteBehavior.SetNull); // Opci�n para mantener las im�genes si se elimina un producto

            modelBuilder.Entity<Imagen>()
                .HasOne(i => i.User)
                .WithOne() // Aseg�rate de que esto est� relacionado con la propiedad de navegaci�n si la tienes
                .HasForeignKey<Imagen>(i => i.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina un usuario, tambi�n se eliminar� la imagen

            modelBuilder.Entity<Imagen>()
                .HasOne(i => i.Categoria)
                .WithMany() // Si no tienes una propiedad de colecci�n en Categoria, esto es correcto
                .HasForeignKey(i => i.CategoriaId)
                .OnDelete(DeleteBehavior.SetNull); // Opci�n para mantener la imagen si se elimina una categor�a
           
            // Relacion muchos a muchos usando la tabla intermedia. ProductoCategoria
            modelBuilder.Entity<ProductoCategoria>()
                .HasKey(pc => new { pc.idProducto, pc.idCategoria });

            modelBuilder.Entity<ProductoCategoria>()
                .HasOne(pc => pc.Producto)
                .WithMany(p => p.CategoriaProductos)
                .HasForeignKey(pc => pc.idProducto);

            modelBuilder.Entity<ProductoCategoria>()
                .HasOne(pc => pc.Categoria)
                .WithMany(c => c.CategoriaProductos)
                .HasForeignKey(pc => pc.idCategoria);


            
        }

    }
}
