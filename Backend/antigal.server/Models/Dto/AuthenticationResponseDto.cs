namespace antigal.server.Models.Dto
{
    public class AuthenticationResponseDto
    {
        public bool? IsAuthSuccesful {  get; set; }
        public string? ErrorMessage { get; set; }
        public string? Token { get; set; }
    }
}
