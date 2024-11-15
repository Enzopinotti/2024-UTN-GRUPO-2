using System;
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
            var envio = await _context.Envios.FindAsync(id);
            if (envio == null)
            {
                throw new KeyNotFoundException($"No se encontró el envío con ID {id}.");
            }
            return envio;
        }

        public async Task<Envio> AddAsync(Envio envio)
        {
            _context.Envios.Add(envio);
            await _context.SaveChangesAsync();
            return envio;
        }

        public async Task<Envio> UpdateAsync(Envio envio)
        {
            var existingEnvio = await _context.Envios.FindAsync(envio.Id);
            if (existingEnvio == null)
            {
                throw new KeyNotFoundException($"No se encontró el envío con ID {envio.Id}.");
            }

            existingEnvio.Destinatario = envio.Destinatario;
            existingEnvio.Direccion = envio.Direccion;
            existingEnvio.FechaEnvio = envio.FechaEnvio;

            _context.Envios.Update(existingEnvio);
            await _context.SaveChangesAsync();
            return existingEnvio;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var envio = await _context.Envios.FindAsync(id);
            if (envio == null)
            {
                return false;
            }

            _context.Envios.Remove(envio);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}