namespace antigal.server.Models.Dto
{
    public class CarritoItemDto
    {
        public int idProducto { get; set; }
        public int cantidad { get; set; }
        public decimal precioUnitario { get; set; }
        public decimal subtotal => precioUnitario * cantidad; // Calcula el subtotal para el elemento
    }
}
