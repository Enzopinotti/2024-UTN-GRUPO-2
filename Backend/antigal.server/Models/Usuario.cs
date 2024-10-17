namespace antigal.server.Models
{
    public class Usuario
    {
        public int idUsuario { get; set; }
        public required string nombreCompleto { get; set; }
        public required string correo { get; set; }
        public required string contrasenia { get; set; }
    }
}
