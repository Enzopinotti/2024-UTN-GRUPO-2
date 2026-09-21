using antigal.server.Models;
using antigal.server.Repositories;

namespace antigal.server.Services
{
    public class ContactoService : IContactoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ContactoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<Contacto> CreateAsync(Contacto contacto)
        {
            contacto.Fecha = DateTime.Now;
            return _unitOfWork.Contactos.AddAsync(contacto);
        }

        public Task<List<Contacto>> GetAllAsync()
        {
            return _unitOfWork.Contactos.GetAllAsync();
        }

        public Task<Contacto?> GetByIdAsync(int id)
        {
            return _unitOfWork.Contactos.GetByIdAsync(id);
        }
    }
}
