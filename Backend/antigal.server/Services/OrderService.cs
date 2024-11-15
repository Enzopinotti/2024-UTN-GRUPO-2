// Services/OrderService.cs
using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using antigal.server.Models.Dto.VentaDtos;
using antigal.server.Controllers;
using Microsoft.AspNetCore.Identity;

namespace antigal.server.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IProductService _productService;

        public OrderService(IUnitOfWork unitOfWork, UserManager<User> userManager, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _productService = productService;
        }

        public async Task<List<Orden>> GetAllOrdersAsync()
        {
            return await _unitOfWork.Orders.GetAllOrdersAsync();
        }

        public async Task<Orden?> GetOrderByIdAsync(int orderId)
        {
            return await _unitOfWork.Orders.GetOrderByIdAsync(orderId);
        }

        public async Task<List<Orden>> GetOrdersByUserIdAsync(string userId)
        {
            return await _unitOfWork.Orders.GetOrdersByUserIdAsync(userId);
        }

        public async Task<List<Orden>> GetOrdersByStatusAsync(string status)
        {
            return await _unitOfWork.Orders.GetOrdersByStatusAsync(status);
        }

        public async Task<Orden?> GetPendingOrderByUserIdAsync(string userId)
        {
            return await _unitOfWork.Orders.GetPendingOrderByUserIdAsync(userId);
        }

        public async Task ConfirmOrder(OrdenDto orderDto)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(orderDto.idUsuario);
                    if (user == null)
                    {
                        throw new KeyNotFoundException($"No se encontró el usuario con ID: {orderDto.idUsuario}.");
                    }

                    var orden = await _unitOfWork.Orders.GetPendingOrderByUserIdAsync(orderDto.idUsuario);
                    if (orden == null)
                    {
                        throw new InvalidOperationException("No se encontró una orden pendiente para este usuario.");
                    }

                    foreach (var itemDto in orderDto.Items)
                    {
                        var response = await _productService.GetProductByIdAsync(itemDto.idProducto);
                        if (!response.IsSuccess || response.Data == null)
                        {
                            throw new KeyNotFoundException($"No se encontró el producto con ID: {itemDto.idProducto}.");
                        }

                        var producto = (Producto)response.Data;

                        if (producto.stock < itemDto.cantidad)
                        {
                            throw new InvalidOperationException($"Stock insuficiente para el producto ID: {itemDto.idProducto}.");
                        }
                    }

                    orden.estado = "Confirmada";
                    var updateResult = await _unitOfWork.Orders.UpdateOrderStatusAsync(orden.idOrden, orden.estado);
                    if (!updateResult)
                    {
                        throw new Exception("No se pudo actualizar el estado de la orden.");
                    }

                    if (orden.Items == null)
                    {
                        throw new Exception("La orden no contiene items.");
                    }

                    decimal total = orden.Items.Sum(item => item.cantidad * (item.Producto?.precio ?? 0));

                    var sale = new Sale
                    {
                        idUsuario = orderDto.idUsuario,
                        idOrden = orden.idOrden,
                        Orden = orden,
                        total = total,
                        metodoPago = "default",
                        fechaVenta = DateTime.Now,
                        EstadoVenta = VentaEstado.Pendiente
                    };

                    var saleCreated = await _unitOfWork.Sales.CreateSaleAsync(sale);
                    if (saleCreated == null)
                    {
                        throw new Exception("Error al realizar la venta, la venta no fue creada");
                    }

                    var saleResponse = new SaleResponseDto
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

                    foreach (var item in orden.Items)
                    {
                        if (item.Producto == null)
                        {
                            throw new Exception($"El producto en la orden con ID {item.idProducto} es nulo.");
                        }

                        item.Producto.stock -= item.cantidad;
                        var updateProductResult = await _unitOfWork.Products.UpdateProductAsync(item.Producto);
                        if (!updateProductResult)
                        {
                            throw new Exception($"No se pudo actualizar el stock del producto ID: {item.Producto.idProducto}.");
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            return await _unitOfWork.Orders.UpdateOrderStatusAsync(orderId, newStatus);
        }
    }
}