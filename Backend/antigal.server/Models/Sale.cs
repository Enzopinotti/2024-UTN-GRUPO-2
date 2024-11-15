using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace antigal.server.Models
{
    public class Sale
    {
        [Key]
        public int idVenta { get; set; }

        [Required]
        public int idOrden { get; set; }
        public required Orden Orden { get; set; }  // Relación con la orden
        public DateTime fechaVenta { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal total { get; set; }  // Precio total con precisión definida
        public required string metodoPago { get; set; }

        // Propiedad que usa el enum para representar el estado de la venta
        public VentaEstado EstadoVenta { get; set; }  // Estado de la venta como enum
        public required string idUsuario { get; set; }

        public User? User { get; set; }      // Propiedad de navegación para User

        // Constructor opcional
        public Sale(int idVenta, int idOrden, Orden orden, DateTime fechaVenta, decimal total, string metodoPago, VentaEstado estadoVenta, string idUsuario)
        {
            this.idVenta = idVenta;
            this.idOrden = idOrden;
            Orden = orden;
            this.fechaVenta = fechaVenta;
            this.total = total;
            this.metodoPago = metodoPago;
            this.EstadoVenta = estadoVenta;
            this.idUsuario = idUsuario;
        }

        // Constructor sin parámetros
        public Sale() { }
    }
}
