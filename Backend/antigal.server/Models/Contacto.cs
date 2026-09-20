namespace antigal.server.Models
{
    public class Contacto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Asunto { get; set; }
        public required string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
    }
}
