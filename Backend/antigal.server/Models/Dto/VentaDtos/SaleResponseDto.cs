namespace antigal.server.Models.Dto.VentaDtos
{
    public class SaleResponseDto
    {
        public bool IsSuccess { get; set; }
        public required string Message { get; set; }
        public SaleDto ?Data { get; set; }
    }
}
