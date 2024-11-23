namespace antigal.server.Models.Dto.VentaDtos
{
    public class SaleRequestDto
    {
        public required string idUsuario { get; set; }
        public int idOrden { get; set; }
        public decimal total { get; set; }
        public required string metodoPago { get; set; }
    }
}
