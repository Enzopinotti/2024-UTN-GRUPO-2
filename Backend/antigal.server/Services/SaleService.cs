// Services/SaleService.cs
using antigal.server.Models.Dto.VentaDtos;
using antigal.server.Models.Dto;
using antigal.server.Models;
using antigal.server.Repositories;
using System;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public class SaleService : ISaleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SaleResponseDto> CreateSaleAsync(string userId, int idOrden, decimal total, string metodoPago)
        {
            try
            {
                var orden = await _unitOfWork.Orders.GetOrderByIdAsync(idOrden);
                if (orden == null)
                {
                    throw new KeyNotFoundException($"Orden no encontrada con ID: {idOrden}");
                }

                var sale = new Sale
                {
                    idOrden = idOrden,
                    Orden = orden,
                    fechaVenta = DateTime.Now,
                    total = total,
                    metodoPago = metodoPago,
                    EstadoVenta = VentaEstado.Pendiente,
                    idUsuario = userId
                };

                var saleCreated = await _unitOfWork.Sales.CreateSaleAsync(sale);
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
                        estado = saleCreated.EstadoVenta.ToString(),
                        idUsuario = saleCreated.idUsuario
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al intentar crear la venta", ex);
            }
        }

        public async Task<SaleDto?> GetSaleByIdAsync(int idVenta)
        {
            try
            {
                var sale = await _unitOfWork.Sales.GetSaleByIdAsync(idVenta);
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
                    estado = sale.EstadoVenta.ToString(),
                    idUsuario = sale.idUsuario
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al intentar obtener la venta", ex);
            }
        }

        public async Task<bool> UpdateSaleStatusAsync(int idVenta, VentaEstado nuevoEstado)
        {
            var sale = await _unitOfWork.Sales.GetSaleByIdAsync(idVenta);
            if (sale == null)
            {
                return false;
            }

            sale.EstadoVenta = nuevoEstado;
            var result = await _unitOfWork.Sales.UpdateSaleAsync(sale);
            if (result)
            {
                await _unitOfWork.SaveChangesAsync();
            }
            return result;
        }
    }
}