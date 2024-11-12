namespace antigal.server.Models
{
    public class RefreshToken
    {
        public int Id { get; set; } // Identificador único
        public required string Token { get; set; } // El refresh token
        public required string UserId { get; set; } // ID del usuario al que pertenece
        public DateTime Expiration { get; set; } // Fecha de expiración del refresh token
    }
}
