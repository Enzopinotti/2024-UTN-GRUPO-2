namespace antigal.server.Models.Dto
{
    public class ProductoCreateDTO
    {
        public required string Nombre { get; set; }
        public required string Marca { get; set; }
        public string? Descripcion { get; set; }
        public string? CodigoBarras { get; set; }
        public int? Disponible { get; set; }    // Si el producto está disponible
        public int? Destacado { get; set; }     // Si el producto es destacado
        public decimal Precio { get; set; }
        public int Stock { get; set; }

        // IDs de las categorías seleccionadas
        public List<int>? CategoriaIds { get; set; }

        // Imágenes a subir (se manejan como archivos para Cloudinary)
        public List<IFormFile>? Imagenes { get; set; }
    }
}
