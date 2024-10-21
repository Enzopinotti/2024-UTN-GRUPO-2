using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System;

namespace antigal.server.Models
{
    public class Imagen
    {
        [Key]
        public int Id { get; set; }
        public required string Url { get; set; }
        public int? ProductoId { get; set; }
        public string? UsuarioId { get; set; }
        public int? CategoriaId { get; set; }
        public DateTime FechaSubida { get; set; } = DateTime.UtcNow;

        // Relación con otras entidades
        public Producto? Producto { get; set; }
        public User? User { get; set; }
        public Categoria? Categoria { get; set; }
        
    }
}
