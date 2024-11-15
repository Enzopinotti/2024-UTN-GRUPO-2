namespace antigal.server.Models.Dto.CarritoDtos
{
    public class AgregarCarritoItemDto
    {
        public int idProducto { get; set; }
        public string? nombreProducto { get; set; }
        public int cantidad { get; set; }
    }
}
