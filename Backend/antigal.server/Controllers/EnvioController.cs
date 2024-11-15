using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace antigal.server.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class EnvioController : ControllerBase
    {
        private readonly IEnvioService _envioService;
        private readonly ILogger<EnvioController> _logger;

        public EnvioController(IEnvioService envioService, ILogger<EnvioController> logger)
        {
            _envioService = envioService;
            _logger = logger;
        }

        // Obtener todos los envíos
        [AllowAnonymous]
        [HttpGet("getEnvios")]
        public async Task<ActionResult<ResponseDto>> GetEnvios()
        {
            _logger.LogInformation("Solicitando lista de todos los envíos.");
            var response = await _envioService.GetEnviosAsync();
            if (response.IsSuccess)
            {
                _logger.LogInformation("Se retornaron envíos exitosamente.");
                return Ok(response);
            }
            _logger.LogError("Error al obtener envíos: {Mensaje}", response.Message);
            return BadRequest(response);
        }

        // Obtener un envío por ID
        [AllowAnonymous]
        [HttpGet("getEnvioById/{id}")]
        public async Task<ActionResult<ResponseDto>> GetEnvioById(int id)
        {
            _logger.LogInformation("Solicitando envío con ID {EnvioId}.", id);
            var response = await _envioService.GetEnvioByIdAsync(id);
            if (response.IsSuccess)
            {
                _logger.LogInformation("Envío encontrado con ID {EnvioId}.", id);
                return Ok(response);
            }
            _logger.LogWarning("Envío con ID {EnvioId} no encontrado: {Mensaje}", id, response.Message);
            return NotFound(response);
        }

        // Agregar un nuevo envío
        [HttpPost("addEnvio")]
        public async Task<ActionResult<ResponseDto>> AddEnvio([FromBody] Envio envio)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Datos de envío inválidos proporcionados.");
                return BadRequest(new ResponseDto { IsSuccess = false, Message = "Datos de envío inválidos." });
            }

            _logger.LogInformation("Intentando agregar un nuevo envío para {Destinatario}.", envio.Destinatario);
            var response = await _envioService.AddEnvioAsync(envio);
            if (response.IsSuccess)
            {
                _logger.LogInformation("Envío agregado exitosamente con ID {EnvioId}.", envio.Id);
                return CreatedAtAction(nameof(GetEnvioById), new { id = envio.Id }, response);
            }
            _logger.LogError("Error al agregar envío: {Mensaje}", response.Message);
            return BadRequest(response);
        }

        // Actualizar un envío existente
        [HttpPut("updateEnvio/{id}")]
        public async Task<ActionResult<ResponseDto>> UpdateEnvio(int id, [FromBody] Envio envio)
        {
            if (id != envio.Id)
            {
                _logger.LogWarning("ID de envío en la URL ({UrlId}) no coincide con el ID en el cuerpo ({BodyId}).", id, envio.Id);
                return BadRequest(new ResponseDto { IsSuccess = false, Message = "El ID del envío no coincide." });
            }

            _logger.LogInformation("Intentando actualizar el envío con ID {EnvioId}.", id);
            var response = await _envioService.UpdateEnvioAsync(envio);
            if (response.IsSuccess)
            {
                _logger.LogInformation("Envío con ID {EnvioId} actualizado exitosamente.", id);
                return NoContent();
            }
            _logger.LogError("Error al actualizar envío: {Mensaje}", response.Message);
            return BadRequest(response);
        }

        // Eliminar un envío
        [HttpDelete("deleteEnvio/{id}")]
        public async Task<ActionResult<ResponseDto>> DeleteEnvio(int id)
        {
            _logger.LogInformation("Intentando eliminar el envío con ID {EnvioId}.", id);
            var response = await _envioService.DeleteEnvioAsync(id);
            if (response.IsSuccess)
            {
                _logger.LogInformation("Envío con ID {EnvioId} eliminado exitosamente.", id);
                return NoContent();
            }
            _logger.LogWarning("No se pudo eliminar el envío con ID {EnvioId}: {Mensaje}", id, response.Message);
            return NotFound(response);
        }
    }
}