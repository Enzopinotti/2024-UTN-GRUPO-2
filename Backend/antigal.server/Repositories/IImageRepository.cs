using antigal.server.Models;

namespace antigal.server.Repositories
{
    public interface IImageRepository
    {
        Task<Imagen> AddAsync(Imagen image);
        Task<Imagen?> GetByIdAsync(int imageId);
        Task<Imagen?> GetByUrlAsync(string imageUrl);
        Task DeleteAsync(Imagen image);
    }
}
