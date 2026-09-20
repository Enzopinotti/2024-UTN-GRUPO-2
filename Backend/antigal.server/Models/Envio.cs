namespace antigal.server.Models
{
    public class Envio
    {
        public int Id { get; set; }
        public required string Destinatario { get; set; }
        public required string Direccion { get; set; }

        public DateTime FechaEnvio { get; set; }
    }
}
