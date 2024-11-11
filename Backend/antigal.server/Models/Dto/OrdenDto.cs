using System.ComponentModel.DataAnnotations;

namespace antigal.server.Models.Dto
{
    public class OrdenDto
    {
        [Required(ErrorMessage = "idUsuario es requerido.")]
        public required string idUsuario { get; set; }  // Cambiar de int a string

        [Required(ErrorMessage = "La lista de ítems es requerida.")]
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();

        public decimal montoTotal { get; set; } // Monto total de la orden
    }

    public class OrderItemDto
    {
        [Required(ErrorMessage = "idProducto es requerido.")]
        public int idProducto { get; set; }

        [Required(ErrorMessage = "cantidad es requerido.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int cantidad { get; set; } = 1; // Valor por defecto de ejemplo
    }
}
