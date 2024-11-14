using antigal.server.Models;

namespace antigal.server.Relationships
{
    public class ProductoCategoria
    {
        public int idProducto { get; set; }
        public Producto? Producto { get; set; }

        public int idCategoria { get; set; }
        public Categoria? Categoria { get; set; }
    }
}
