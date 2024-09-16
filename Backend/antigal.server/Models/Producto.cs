using System.ComponentModel.DataAnnotations;

namespace antigal.server.Models
{
    public class Producto
    {
        [Key]
        public int idProducto { get; set; }
        [Required]
        public string nombre { get; set; }
        [Required]
        public string marca { get; set; }
        public string? descripcion { get; set; }
        public int?  codigoBarras { get; set; }
        [Required]
        public int disponible { get; set; }
        public int? destacado { get; set; }
        [Required]
        public  float precio { get; set; }
        [Required]
        public  int stock { get; set; }
 


        public Producto(int idProducto, string nombre, string marca, string? descripcion, int? codigoBarras, int disponible, int? destacado, float precio, int stock)
        {
            this.idProducto = idProducto;
            this.nombre = nombre;
            this.marca = marca;
            this.descripcion = descripcion;
            this.codigoBarras = codigoBarras;
            this.disponible = disponible;
            this.destacado = destacado;
            this.precio = precio;
            this.stock = stock;
        }

    }
}
