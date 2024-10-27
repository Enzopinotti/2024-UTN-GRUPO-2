namespace antigal.server.Models.Dto;
public class OrderDto
{
    public string UserId { get; set; }  // Cambiar de int a string
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();


    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
