namespace antigal.server.Models.Dto
{
    public class ProductoReadDTO
    {
        public int IdProducto { get; set; }
        public required string Nombre { get; set; }
        public required string Marca { get; set; }
        public string? Descripcion { get; set; }
        public long? CodigoBarras { get; set; }
        public bool Disponible { get; set; }
        public bool Destacado { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        // Devuelve las URLs y los publicId de las imágenes relacionadas
        public List<ImagenDTO> Imagenes { get; set; } = new List<ImagenDTO>();

        // Devuelve los nombres de las categorías relacionadas
        public List<string> Categorias { get; set; } = new List<string>();
    }
}
