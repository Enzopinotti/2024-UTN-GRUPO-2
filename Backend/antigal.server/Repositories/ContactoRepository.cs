using antigal.server.Data;
using antigal.server.Models;
using Microsoft.EntityFrameworkCore;

namespace antigal.server.Repositories
{
    public class ContactoRepository : IContactoRepository
    {
        private readonly AppDbContext _context;

        public ContactoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Contacto> AddAsync(Contacto contacto)
        {
            _context.Contactos.Add(contacto);
            await _context.SaveChangesAsync();
            return contacto;
        }

        public Task<List<Contacto>> GetAllAsync()
        {
            return _context.Contactos.ToListAsync();
        }

        public async Task<Contacto?> GetByIdAsync(int id)
        {
            return await _context.Contactos.FindAsync(id);
        }
    }
}
