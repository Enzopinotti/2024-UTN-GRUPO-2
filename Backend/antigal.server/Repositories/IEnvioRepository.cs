using antigal.server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace antigal.server.Repositories
{
    public interface IEnvioRepository
    {
        Task<IEnumerable<Envio>> GetAllAsync();
        Task<Envio> GetByIdAsync(int id);
        Task<Envio> AddAsync(Envio envio);
        Task<Envio> UpdateAsync(Envio envio);
        Task<bool> DeleteAsync(int id);
    }
}