namespace antigal.server.Models
{
    public class Contacto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Asunto { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; }
    }
}
