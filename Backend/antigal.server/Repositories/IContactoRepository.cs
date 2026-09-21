using antigal.server.Models;

namespace antigal.server.Repositories
{
    public interface IContactoRepository
    {
        Task<Contacto> AddAsync(Contacto contacto);
        Task<List<Contacto>> GetAllAsync();
        Task<Contacto?> GetByIdAsync(int id);
    }
}
