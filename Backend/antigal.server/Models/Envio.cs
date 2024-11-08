namespace antigal.server.Models
{
    public class Envio
    {
        public int Id { get; set; }
        public string Destinatario { get; set; }
        public string Direccion { get; set; }

        public  DateTime FechaEnvio { get; set; }
    }
}
