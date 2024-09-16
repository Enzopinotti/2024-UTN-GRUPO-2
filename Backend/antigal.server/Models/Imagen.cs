using System.ComponentModel.DataAnnotations;

namespace antigal.server.Models
{
    public class Imagen
    {
        //[Key] Utilizacion de EF, estableciendo clave primaria, como tambien las columnas requeridas [Required]
        [Key]
        public int id { get; set; }
        [Required]
        public required string url { get; set; }

        // Clave externa que hace referencia a Producto. Este atributo va a ser la [ForeignKey] que se especifica en AppDbContext.cs

        public int idProducto { get; set; }
        // Propiedad de navegación para la relación muchos a uno

        public Producto? Producto { get; set; }

        public Imagen (int id, string url, int idProducto)
        {
            this.id = id;
            this.url = url;
            this.idProducto = idProducto;
        }

        //CONSTRUCTOR SIN PARAMETROS REQUERIDO POR EF.
        public Imagen() {}

    }
}
