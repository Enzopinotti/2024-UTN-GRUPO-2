using antigal.server.Models;
using antigal.server.Services;
using Microsoft.AspNetCore.Mvc;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnvioController : ControllerBase
    {
        private readonly EnvioService _envioService;

        public EnvioController(EnvioService envioService)
        {
            _envioService = envioService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Envio>>> GetEnvios()
        {
            return Ok(await _envioService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Envio>> GetEnvio(int id)
        {
            var envio = await _envioService.GetByIdAsync(id);
            if (envio == null) return NotFound();
            return Ok(envio);
        }

        [HttpPost]
        public async Task<ActionResult<Envio>> PostEnvio(Envio envio)
        {
            var newEnvio = await _envioService.AddAsync(envio);
            return CreatedAtAction(nameof(GetEnvio), new { id = newEnvio.Id }, newEnvio);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEnvio(int id, Envio envio)
        {
            if (id != envio.Id) return BadRequest();
            await _envioService.UpdateAsync(envio);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnvio(int id)
        {
            var success = await _envioService.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
