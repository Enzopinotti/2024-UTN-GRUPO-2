using antigal.server.Models;
using antigal.server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace antigal.server.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactoController : ControllerBase
    {
        private readonly IContactoService _contactoService;

        public ContactoController(IContactoService contactoService)
        {
            _contactoService = contactoService;
        }

        // POST: api/Contacto
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<Contacto>> PostContacto(Contacto contacto)
        {
            var created = await _contactoService.CreateAsync(contacto);

            return CreatedAtAction(nameof(GetContacto), new { id = created.Id }, created);
        }

        // GET: api/Contacto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contacto>>> GetContactos()
        {
            return await _contactoService.GetAllAsync();
        }

        // GET: api/Contacto/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contacto>> GetContacto(int id)
        {
            var contacto = await _contactoService.GetByIdAsync(id);

            if (contacto == null)
            {
                return NotFound();
            }

            return contacto;
        }
    }
}
