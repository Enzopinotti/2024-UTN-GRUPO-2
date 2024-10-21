using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace antigal.server.Models
{
    public class Carrito
    {
        [Key]
        public int idCarrito { get; set; }

        [Required] // Asegura que un carrito siempre tenga un usuario asociado
        public string idUsuario { get; set; } // Identificador del usuario que posee el carrito

        public List<CarritoItem> Items { get; set; } = new List<CarritoItem>(); // Lista de ítems en el carrito

        // Constructor
        public Carrito(string userId)
        {
            idUsuario = userId;
        }

        // Método para calcular el precio total del carrito
        public float CalcularPrecioTotal()
        {
            float total = 0;
            foreach (var item in Items)
            {
                total += item.CalcularSubtotal(); // Llama al método de CarritoItem
            }
            return total;
        }
    }
}
