namespace antigal.server.Models.Dto
{
    public class ImagenDTO
    {
        public int Id { get; set; }
        public string Url { get; set; }         // URL de la imagen en Cloudinary
        public string PublicId { get; set; }    // Identificador de la imagen en Cloudinary
        public DateTime FechaSubida { get; set; }
    }
}
