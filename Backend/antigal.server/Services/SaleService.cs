using antigal.server.Models.Dto;
using antigal.server.Models;
using antigal.server.Repositories;
using antigal.server.Models.Dto.VentaDtos;

namespace antigal.server.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;  // Repositorio de ventas
        private readonly IOrderService _orderService;  // Servicio para obtener la orden

        public SaleService(ISaleRepository saleRepository, IOrderService orderService)
        {
            _saleRepository = saleRepository;
            _orderService = orderService;
        }

        public async Task<SaleResponseDto> CreateSaleAsync(string userId, int idOrden, decimal total, string metodoPago)
        {
            try
            {
                // Obtener la orden asociada
                var orden = await _orderService.GetOrderByIdAsync(idOrden);

                if (orden == null)
                {
                    throw new KeyNotFoundException($"Orden no encontrada con ID: {idOrden}");
                }

                // Crear la venta
                var sale = new Sale
                {
                    idOrden = idOrden,
                    Orden = orden,
                    fechaVenta = DateTime.Now,
                    total = total,
                    metodoPago = metodoPago,
                    estado = "Pendiente",
                    idUsuario = userId
                };

                var saleCreated = await _saleRepository.CreateSaleAsync(sale);

                if (saleCreated == null)
                {
                    throw new InvalidOperationException("Error al realizar la venta, la venta no fue creada");
                }

                return new SaleResponseDto
                {
                    IsSuccess = true,
                    Message = "Venta realizada con éxito",
                    Data = new SaleDto
                    {
                        idVenta = saleCreated.idVenta,
                        idOrden = saleCreated.idOrden,
                        fechaVenta = saleCreated.fechaVenta,
                        total = saleCreated.total,
                        metodoPago = saleCreated.metodoPago,
                        estado = saleCreated.estado,
                        idUsuario = saleCreated.idUsuario
                    }
                };
            }
            catch (Exception ex)
            {
                // Registrar el error en logs si fuera necesario
                throw new Exception("Ocurrió un error al intentar crear la venta", ex);
            }
        }

        public async Task<SaleDto?> GetSaleByIdAsync(int idVenta)
        {
            try
            {
                var sale = await _saleRepository.GetSaleByIdAsync(idVenta);

                if (sale == null)
                {
                    throw new KeyNotFoundException($"No se encontró la venta con ID: {idVenta}");
                }

                return new SaleDto
                {
                    idVenta = sale.idVenta,
                    idOrden = sale.idOrden,
                    fechaVenta = sale.fechaVenta,
                    total = sale.total,
                    metodoPago = sale.metodoPago,
                    estado = sale.estado,
                    idUsuario = sale.idUsuario
                };
            }
            catch (Exception ex)
            {
                // Registrar el error en logs si fuera necesario
                throw new Exception("Ocurrió un error al intentar obtener la venta", ex);
            }
        }

    }
}
