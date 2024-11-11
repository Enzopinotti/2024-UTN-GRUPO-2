using antigal.server.Models.Dto.VentaDtos;
using antigal.server.Models.Dto;

namespace antigal.server.Services
{
    public interface ISaleService
    {
        Task<SaleResponseDto> CreateSaleAsync(string userId, int idOrden, decimal total, string metodoPago);
        Task<SaleDto?> GetSaleByIdAsync(int idVenta);
    }
}


// 6/11 2am 
// agregar mas funciones, como por ejemplo, traer todas las ventas (funcion para el admin), ordenar por preciototal, fecha, etc, de venta (funcion admin)
// verificar que anda todo lo de venta, las conexiones entre orden y venta, orden y carrito.