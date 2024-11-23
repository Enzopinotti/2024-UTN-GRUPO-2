// Services/ISaleService.cs
using antigal.server.Models.Dto.VentaDtos;
using antigal.server.Models.Dto;
using antigal.server.Models;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public interface ISaleService
    {
        Task<SaleResponseDto> CreateSaleAsync(string userId, int idOrden, decimal total, string metodoPago);
        Task<SaleDto?> GetSaleByIdAsync(int idVenta);
        Task<bool> UpdateSaleStatusAsync(int idVenta, VentaEstado nuevoEstado);
    }
}