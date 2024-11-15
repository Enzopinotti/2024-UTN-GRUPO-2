namespace antigal.server.Models.Dto
{
    public class AuthResponseDto
    {
        public bool IsAuthSuccesful { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; } // Nuevo campo para el refresh token
        public string? ErrorMessage { get; set; }
    }


}
