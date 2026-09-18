using antigal.server.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Order
{
    public int Id { get; set; }  // Clave primaria de la orden

    [Required]
    public string UserId { get; set; }  // Clave foránea hacia el usuario

    [ForeignKey("UserId")]
    public User User { get; set; }  // Relación con IdentityUser

    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = "Pending";

    public List<OrderItem> Items { get; set; } = new List<OrderItem>();  // Relación con los ítems de la orden
}
