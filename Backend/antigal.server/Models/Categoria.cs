using antigal.server.Relationships;
using System.ComponentModel.DataAnnotations;

namespace antigal.server.Models
{
    public class Categoria
    {
        [Key]
        public int idCategoria { get; set; }
        public string? descripcion { get; set; }
        public string? imagen { get; set; }
        [Required]
        public required string nombre { get; set; }

        // RELACION MUCHOS A MUCHOS CON PRODUCTO
        public List<ProductoCategoria> CategoriaProductos { get; set; } = new List<ProductoCategoria>();

        public Categoria(int idCategoria, string? descripcion, string? imagen, string nombre)
                {
                    this.idCategoria = idCategoria;
                    this.descripcion = descripcion;
                    this.imagen = imagen;
                    this.nombre = nombre;
                }

        public Categoria() { }
    }
}
