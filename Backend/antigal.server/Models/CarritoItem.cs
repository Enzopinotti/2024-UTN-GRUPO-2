using System.ComponentModel.DataAnnotations;

namespace antigal.server.Models
{
    public class CarritoItem
    {
        [Key]
        public int idCarritoItem { get; set; }  // ID del ítem en el carrito

        public int idProducto { get; set; } // ID del producto
        public Producto? Producto { get; set; } // Producto asociado (puede ser nulo)

        public int cantidad { get; set; }   // Cantidad del producto en el carrito

        // Método para calcular el precio total del ítem
        public float CalcularSubtotal()
        {
            return (Producto != null) ? Producto.precio * cantidad : 0; // Retorna 0 si el producto es nulo
        }
    }
}
