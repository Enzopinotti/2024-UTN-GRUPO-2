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
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoItem> CarritoItems { get; set; }

        //OnModelCreating se utiliza para establecer las asociaciones entre dos clases para que impacten en la base de datos desde .NET
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Llama al m�todo base

            modelBuilder.Entity<Producto>()
                .HasMany(p => p.imagenes)  //Un producto tiene muchas imagenes
                .WithOne(i => i.Producto)  //Una imagen pertenece a un producto
                .HasForeignKey(i => i.idProducto) //Establece la FK
                .OnDelete(DeleteBehavior.Cascade); //Establece la regla la cual dice que si se elimina un Producto, se elmininaran las imagenes de dicho producto.
       
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

            modelBuilder.Entity<Carrito>()
                .HasMany(c => c.Items) // Un Carrito tiene muchos CarritoItems
                .WithOne() // Cada CarritoItem se relaciona con un Carrito
                .HasForeignKey(ci => ci.idCarritoItem); // Aqu� se asume que idCarritoItem en CarritoItem es la clave for�nea (ajusta si es necesario)

            modelBuilder.Entity<CarritoItem>()
                .HasOne(ci => ci.Producto) // Cada CarritoItem tiene un Producto
                .WithMany() // Un Producto puede estar en muchos CarritoItems
                .HasForeignKey(ci => ci.idProducto); // Asumiendo que tienes una propiedad idProducto en CarritoItem

        }

    }
}
