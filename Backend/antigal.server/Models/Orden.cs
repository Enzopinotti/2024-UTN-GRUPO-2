using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace antigal.server.Models
{
    public class Orden
    {
        [Key]
        public int idOrden { get; set; }  // Clave primaria de la orden

        [Required]
        public required string idUsuario { get; set; }  // Clave foránea hacia el usuario

        [ForeignKey("idUsuario")]
        public required User User { get; set; }  // Relación con IdentityUser

        public DateTime fechaOrden { get; set; }
        public string estado { get; set; } = "Pendiente";

        public List<OrdenDetalle> Items { get; set; } = new List<OrdenDetalle>();  // Relación con los ítems de la orden
    }
}
