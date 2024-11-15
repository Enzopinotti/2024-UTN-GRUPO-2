namespace antigal.server.Models
{
    public class Like
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int ProductoId { get; set; }


        // Propiedad de navegación hacia el usuario
        public User? User { get; set; }
    }
}
