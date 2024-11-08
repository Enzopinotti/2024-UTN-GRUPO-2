using antigal.server.Models;
using antigal.server.Repositories;

namespace antigal.server.Services
{
    public class EnvioService
    {
        private readonly IEnvioRepository _envioRepository;

        public EnvioService(IEnvioRepository envioRepository)
        {
            _envioRepository = envioRepository;
        }

        public Task<IEnumerable<Envio>> GetAllAsync() => _envioRepository.GetAllAsync();

        public Task<Envio> GetByIdAsync(int id) => _envioRepository.GetByIdAsync(id);

        public Task<Envio> AddAsync(Envio envio) => _envioRepository.AddAsync(envio);

        public Task<Envio> UpdateAsync(Envio envio) => _envioRepository.UpdateAsync(envio);

        public Task<bool> DeleteAsync(int id) => _envioRepository.DeleteAsync(id);
    }
}
