using antigal.server.Models;

namespace antigal.server.Services
{
    public interface IContactoService
    {
        Task<Contacto> CreateAsync(Contacto contacto);
        Task<List<Contacto>> GetAllAsync();
        Task<Contacto?> GetByIdAsync(int id);
    }
}
