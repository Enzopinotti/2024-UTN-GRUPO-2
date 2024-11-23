namespace antigal.server.Models.Dto
{
    public class CarritoDto
    {
        public int idCarrito { get; set; }
        public List<CarritoItemDto> Items { get; set; } = new List<CarritoItemDto>();
        public decimal total => Items.Sum(item => item.subtotal); // Calcula el total del carrito
    }
}
