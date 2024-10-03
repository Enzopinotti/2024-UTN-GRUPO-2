using antigal.server.Relationships;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace antigal.server.Models
{
    public class Categoria
    {
        [Key]
        public int idCategoria { get; set; }
        public required string nombre { get; set; }
        public string? descripcion { get; set; }
        public string? imagen { get; set; }

        // RELACION MUCHOS A MUCHOS CON PRODUCTO
        [JsonIgnore]
        public List<ProductoCategoria> CategoriaProductos { get; set; } = new List<ProductoCategoria>();

        public Categoria(int idCategoria, string nombre, string? descripcion, string? imagen)
        {
            this.idCategoria = idCategoria;
            this.nombre = nombre;
            this.descripcion = descripcion;
            this.imagen = imagen;
        }
        public Categoria() { }
    }
}
