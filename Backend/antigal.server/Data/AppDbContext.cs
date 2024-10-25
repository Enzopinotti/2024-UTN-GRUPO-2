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

            modelBuilder.Entity<Categoria>()
                .HasMany(c => c.CategoriaProductos)
                .WithOne()
                .HasForeignKey(cp => cp.idCategoria); 

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

            ///////////////////////Imagenes/////////////////////////
            modelBuilder.Entity<Imagen>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(i => i.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
            // Relación uno a muchos entre Carrito y CarritoItem
            modelBuilder.Entity<Carrito>()
                .HasMany(c => c.Items) // Un carrito tiene muchos items
                .WithOne()
                .HasForeignKey(ci => ci.idCarrito) // FK en CarritoItem
                .OnDelete(DeleteBehavior.Cascade); // Si se elimina el carrito, se eliminan sus items

            modelBuilder.Entity<Imagen>()
                .HasOne<Categoria>()
                .WithMany()
                .HasForeignKey(i => i.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Imagen>()
                .HasOne<Producto>()
                .WithMany()
                .HasForeignKey(i => i.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);


            // Relación uno a muchos entre CarritoItem y Producto
            modelBuilder.Entity<CarritoItem>()
                .HasOne(ci => ci.Producto) // Cada CarritoItem tiene un Producto
                .WithMany() // Un Producto puede estar en muchos CarritoItems
                .HasForeignKey(ci => ci.idProducto) // FK en CarritoItem
                .OnDelete(DeleteBehavior.Restrict); // No eliminar el producto si tiene items en carritos

        }

    }
}
