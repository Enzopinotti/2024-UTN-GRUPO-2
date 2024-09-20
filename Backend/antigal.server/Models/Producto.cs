using antigal.server.Relationships;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace antigal.server.Models
{
    public class Producto
    {
        [Key]
        public int idProducto { get; set; }
        [Required]
        public required string nombre { get; set; }
        [Required]
        public required string marca { get; set; }
        public string? descripcion { get; set; }
        public int?  codigoBarras { get; set; }
        [Required]
        public int disponible { get; set; }
        public int? destacado { get; set; }
        [Required]
        public  float precio { get; set; }
        [Required]
        public  int stock { get; set; }

        //RELACION UNO A MUCHOS CON IMAGEN
       
        public List <Imagen> imagenes { get; set; } = new List<Imagen> ();

        //RELACION MUCHOS A MUCHOS CON CATEGORIA



        //REVISAR
        [JsonIgnore]
        public List<ProductoCategoria> CategoriaProductos { get; set; } = new List<ProductoCategoria>();
        //REVISAR



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
        //CONSTRUCTOR SIN PARAMETROS REQUERIDO POR EF.
        public Producto() {}

    }
}
