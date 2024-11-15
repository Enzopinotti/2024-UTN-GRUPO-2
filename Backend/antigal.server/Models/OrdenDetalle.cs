using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace antigal.server.Models
{
    public class OrdenDetalle
    {
        [Key]
        public int idDetalle { get; set; }  // Clave primaria para el detalle de la orden

        [ForeignKey("Orden")]
        public int idOrdenDetalle { get; set; }  // Clave foránea hacia la orden

        [ForeignKey("Producto")]
        public int idProducto { get; set; }  // Clave foránea hacia el producto

        public int cantidad { get; set; }  // Cantidad del producto en la orden

        public decimal precio { get; set; }  // Precio del producto al momento de la compra

        public Orden ?Orden { get; set; }  // Relación con la orden
        public Producto ?Producto { get; set; }  // Relación con el producto
    }
}
