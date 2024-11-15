using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Repositories;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public class EnvioService : IEnvioService
    {
        private readonly IEnvioRepository _envioRepository;

        public EnvioService(IEnvioRepository envioRepository)
        {
            _envioRepository = envioRepository;
        }

        public async Task<ResponseDto> GetEnviosAsync()
        {
            var response = new ResponseDto();
            try
            {
                var envios = await _envioRepository.GetAllAsync();
                response.Data = envios;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error al obtener envíos: {ex.Message}";
            }
            return response;
        }

        public async Task<ResponseDto> GetEnvioByIdAsync(int id)
        {
            var response = new ResponseDto();
            try
            {
                var envio = await _envioRepository.GetByIdAsync(id);
                response.Data = envio;
                response.IsSuccess = true;
            }
            catch (KeyNotFoundException knfEx)
            {
                response.IsSuccess = false;
                response.Message = knfEx.Message;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error al obtener el envío: {ex.Message}";
            }
            return response;
        }

        public async Task<ResponseDto> AddEnvioAsync(Envio envio)
        {
            var response = new ResponseDto();
            try
            {
                var newEnvio = await _envioRepository.AddAsync(envio);
                response.Data = newEnvio;
                response.Message = "Envío agregado exitosamente.";
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error al agregar el envío: {ex.Message}";
            }
            return response;
        }

        public async Task<ResponseDto> UpdateEnvioAsync(Envio envio)
        {
            var response = new ResponseDto();
            try
            {
                var updatedEnvio = await _envioRepository.UpdateAsync(envio);
                response.Data = updatedEnvio;
                response.Message = "Envío actualizado exitosamente.";
                response.IsSuccess = true;
            }
            catch (KeyNotFoundException knfEx)
            {
                response.IsSuccess = false;
                response.Message = knfEx.Message;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error al actualizar el envío: {ex.Message}";
            }
            return response;
        }

        public async Task<ResponseDto> DeleteEnvioAsync(int id)
        {
            var response = new ResponseDto();
            try
            {
                var success = await _envioRepository.DeleteAsync(id);
                if (success)
                {
                    response.Message = "Envío eliminado exitosamente.";
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = $"No se encontró el envío con ID {id}.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error al eliminar el envío: {ex.Message}";
            }
            return response;
        }
    }
}