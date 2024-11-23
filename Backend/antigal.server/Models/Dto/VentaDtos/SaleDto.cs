namespace antigal.server.Models.Dto
{
    public class SaleDto
    {
        public int idVenta { get; set; }
        public int idOrden { get; set; }
        public DateTime fechaVenta { get; set; }
        public decimal total { get; set; }
        public required string metodoPago { get; set; }
        public required string estado { get; set; }
        public required string idUsuario { get; set; }
    }
}
