namespace antigal.server.Models.Dto
{
    public class RegisterResponseDto
    {
       public bool IsSuccessfulRegistration {  get; set; }
       public IEnumerable<string>? Errors { get; set; }
    }
}
