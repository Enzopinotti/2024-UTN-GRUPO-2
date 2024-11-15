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
        public int?  codigoBarras { get; set; }
        public int? disponible { get; set; }
        public int? destacado { get; set; }
        public decimal precio { get; set; }
        public  int stock { get; set; }

        //RELACION UNO A MUCHOS CON IMAGEN
        public List<string> ImagenUrls { get; set; } = new List<string>(); // Almacena solo las URLs

        //RELACION MUCHOS A MUCHOS CON CATEGORIA   
        [JsonIgnore]
        public List<ProductoCategoria> CategoriaProductos { get; set; } = new List<ProductoCategoria>();


        public Producto(int idProducto, string nombre, string marca, string? descripcion, int? codigoBarras, int disponible, int? destacado, decimal precio, int stock)
        {
            this.idProducto = idProducto;
            this.nombre = nombre;
            this.marca = marca;
            this.descripcion = descripcion;
            this.codigoBarras = codigoBarras;
            this.disponible = verificarDisponible();
            this.destacado = destacado;
            this.precio = precio;
            this.stock = stock;
        }

        //funcion para que actualice automatico el "disponible" (no funciona)
        public int verificarDisponible()
        {
            return stock > 0 ? 1 : 0; // Devuelve 1 si stock > 0, de lo contrario 0
        }

        //CONSTRUCTOR SIN PARAMETROS REQUERIDO POR EF.
        public Producto() {}

        

    }
    
}
