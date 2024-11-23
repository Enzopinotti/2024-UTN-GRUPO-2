namespace antigal.server.Models.Dto
{
    public class AddItemToCartDto
    {
        public int idProducto { get; set; }
        public string ?Nombre { get; set; }
        public int cantidad { get; set; }
    }
}
