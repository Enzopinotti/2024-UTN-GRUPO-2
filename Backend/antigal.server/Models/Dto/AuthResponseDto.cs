namespace antigal.server.Models.Dto
{
    public class AuthResponseDto
    {
        public bool IsAuthSuccesful {  get; set; }
        public string? ErrorMessage { get; set; }
        public string? Token { get; set; }
    }
}
