using antigal.server.Models;
using antigal.server.Relationships;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using antigal.server.SeedConfiguration;
namespace antigal.server.Data
{
    public class AppDbContext : IdentityDbContext<User, Models.Role, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }  // Esta l�nea es el constructor del contexto de la base de datos (AppDbContext).

        //MAPEO PRODUCTOS. Los DbSet se utilizan para agregar las clases que van a ser mapeadas a la base de datos.
        public DbSet <Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet <Imagen> Imagenes { get; set; }
        public DbSet <ProductoCategoria> ProductoCategoria { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoItem> CarritoItems { get; set; }
        public DbSet<Orden> Ordenes { get; set; }
        public DbSet<OrdenDetalle> OrdenDetalle { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; } // Para almacenar refresh tokens
        public DbSet<Like> Likes { get; set; }
        public DbSet<Sale> Sales { get; set; } // DbSet para la entidad Sale
        //OnModelCreating se utiliza para establecer las asociaciones entre dos clases para que impacten en la base de datos desde .NET
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Llama al m�todo base

            modelBuilder.ApplyConfiguration(new RoleConfiguration());
          //  modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
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
            //likes
            modelBuilder.Entity<Like>()
            .HasKey(l => new { l.UserId, l.ProductoId });
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
                                                    // Relación uno a muchos: Orden -> OrdenItems
                                                    // Relación uno a muchos: User -> Ordenes
            modelBuilder.Entity<Orden>()
                 .HasOne(o => o.User)  // Una orden tiene un usuario
                 .WithMany()  // Un usuario puede tener muchas órdenes
                 .HasForeignKey(o => o.idUsuario)  // FK en Orden
                 .OnDelete(DeleteBehavior.Restrict);  // No permitir eliminar un usuario con órdenes

            // Relación uno a muchos: Orden -> OrdenItems
            modelBuilder.Entity<Orden>()
                .HasMany(o => o.Items)  // Una orden tiene muchos ítems
                .WithOne(oi => oi.Orden)  // Cada ítem pertenece a una orden
                .HasForeignKey(oi => oi.idOrdenDetalle)  // FK en OrdenItem
                .OnDelete(DeleteBehavior.Cascade);  // Si se elimina la orden, se eliminan sus ítems

            // Relación uno a muchos: Producto -> OrdenDetalle
            modelBuilder.Entity<OrdenDetalle>()
                .HasOne(oi => oi.Producto)  // Cada ítem tiene un producto
                .WithMany()  // Un producto puede estar en muchos ítems
                .HasForeignKey(oi => oi.idProducto)  // FK en OrdenDetalle
                .OnDelete(DeleteBehavior.Restrict);  // No eliminar producto si está asociado a ítems


            // Configuración para la propiedad precio en OrderDetail
            modelBuilder.Entity<OrdenDetalle>()
                .Property(od => od.precio)
                .HasColumnType("decimal(18,2)");

            // Relación uno a uno entre Orden y Venta
            modelBuilder.Entity<Orden>()
                .HasOne(o => o.Sale)  // Una orden tiene una venta asociada
                .WithOne(s => s.Orden)  // Una venta tiene una orden asociada
                .HasForeignKey<Sale>(s => s.idOrden);  // La clave foránea está en Sale
        }
    }

    }

