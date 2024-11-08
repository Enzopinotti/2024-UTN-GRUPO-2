using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using antigal.server.Models;
using antigal.server.Data;

namespace antigal.server.Repositories
{
    public class EnvioRepository : IEnvioRepository
    {
        private readonly AppDbContext _context;

        public EnvioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Envio>> GetAllAsync()
        {
            return await _context.Envios.ToListAsync();
        }

        public async Task<Envio> GetByIdAsync(int id)
        {
            return await _context.Envios.FindAsync(id);
        }

        public async Task<Envio> AddAsync(Envio envio)
        {
            _context.Envios.Add(envio);
            await _context.SaveChangesAsync();
            return envio;
        }

        public async Task<Envio> UpdateAsync(Envio envio)
        {
            _context.Entry(envio).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return envio;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var envio = await _context.Envios.FindAsync(id);
            if (envio == null) return false;

            _context.Envios.Remove(envio);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
