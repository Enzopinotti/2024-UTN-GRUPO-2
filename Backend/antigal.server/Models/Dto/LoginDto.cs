namespace antigal.server.Models.Dto
{
    public class LoginDto
    {
        public required string UserName { get; set; }   
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
