using System.ComponentModel.DataAnnotations;

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
        public decimal total { get; set; }
        public required string metodoPago { get; set; }
        public string estado { get; set; } = "Pendiente";
        public required string idUsuario { get; set; }

        // Constructor opcional
        public Sale(int idVenta, int idOrden, Orden orden, DateTime fechaVenta, decimal total, string metodoPago, string estado, string idUsuario)
        {
            this.idVenta = idVenta;
            this.idOrden = idOrden;
            Orden = orden;
            this.fechaVenta = fechaVenta;
            this.total = total;
            this.metodoPago = metodoPago;
            this.estado = estado;
            this.idUsuario = idUsuario;
        }

        // Constructor sin parámetros
        public Sale() { }
    }
}
