using antigal.server.Relationships;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace antigal.server.Models
{
    public class Producto
    {
        [Key]
        public int idProducto { get; set; }
        public required string nombre { get; set; }
        public required string marca { get; set; }
        public string? descripcion { get; set; }
        public string?  codigoBarras { get; set; }
        public int? disponible { get; set; }
        public int? destacado { get; set; }
        public decimal precio { get; set; }
        public  int stock { get; set; }

        //RELACION UNO A MUCHOS CON IMAGEs
        public List<Imagen>? Imagenes { get; set; }

        //RELACION MUCHOS A MUCHOS CON CATEGORIA   
        
        public List<ProductoCategoria>? CategoriaProductos { get; set; } = new List<ProductoCategoria>();


        public Producto(int idProducto, string nombre, string marca, string? descripcion, string? codigoBarras, int disponible, int? destacado, decimal precio, int stock)
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
