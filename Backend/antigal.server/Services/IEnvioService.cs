using antigal.server.Models;
using antigal.server.Models.Dto;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public interface IEnvioService
    {
        Task<ResponseDto> GetEnviosAsync();
        Task<ResponseDto> GetEnvioByIdAsync(int id);
        Task<ResponseDto> AddEnvioAsync(Envio envio);
        Task<ResponseDto> UpdateEnvioAsync(Envio envio);
        Task<ResponseDto> DeleteEnvioAsync(int id);
    }
}