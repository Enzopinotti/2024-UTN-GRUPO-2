// Services/ProductService.cs
using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto> GetProductsAsync()
        {
            var response = new ResponseDto();
            try
            {
                var productos = await _unitOfWork.Products.GetAllProductsAsync();
                response.Data = productos;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> GetProductByIdAsync(int id)
        {
            var response = new ResponseDto();
            try
            {
                var producto = await _unitOfWork.Products.GetProductByIdAsync(id);
                if (producto == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Producto no encontrado.";
                    return response;
                }

                response.Data = producto;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> GetProductByTitleAsync(string nombre)
        {
            var response = new ResponseDto();
            try
            {
                var producto = await _unitOfWork.Products.GetProductsByTitleAsync(nombre);
                if (producto == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Producto no encontrado.";
                    return response;
                }

                response.Data = producto;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> AddProductAsync(Producto producto)
        {
            var response = new ResponseDto();
            try
            {
                var productoExistente = await _unitOfWork.Products.GetProductsByTitleAsync(producto.nombre);
                if (productoExistente != null)
                {
                    response.IsSuccess = false;
                    response.Message = "Ya existe un producto con ese nombre.";
                    return response;
                }

                await _unitOfWork.Products.AddProductAsync(producto);
                await _unitOfWork.SaveChangesAsync();

                response.IsSuccess = true;
                response.Message = "Producto agregado exitosamente.";
                response.Data = producto;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> DeleteProductAsync(int id)
        {
            var response = new ResponseDto();
            try
            {
                var producto = await _unitOfWork.Products.GetProductByIdAsync(id);
                if (producto == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Producto no encontrado.";
                    return response;
                }

                await _unitOfWork.Products.DeleteProductAsync(id);
                await _unitOfWork.SaveChangesAsync();

                response.IsSuccess = true;
                response.Message = "Producto eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> PutProductAsync(Producto producto)
        {
            var response = new ResponseDto();
            try
            {
                var productoExistente = await _unitOfWork.Products.GetProductByIdAsync(producto.idProducto);
                if (productoExistente == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Producto no encontrado.";
                    return response;
                }

                productoExistente.nombre = producto.nombre;
                productoExistente.descripcion = producto.descripcion;
                productoExistente.precio = producto.precio;
                productoExistente.stock = producto.stock;
                // Actualiza otras propiedades según sea necesario

                var updateResult = await _unitOfWork.Products.UpdateProductAsync(productoExistente);
                if (!updateResult)
                {
                    response.IsSuccess = false;
                    response.Message = "Error al actualizar el producto.";
                    return response;
                }

                await _unitOfWork.SaveChangesAsync();

                response.IsSuccess = true;
                response.Message = "Producto actualizado exitosamente.";
                response.Data = productoExistente;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> ImportProductsFromExcelAsync(IFormFile file)
        {
            var response = new ResponseDto();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    var productosImportados = await _unitOfWork.Products.ImportProductsFromExcelAsync(stream);
                    response.Data = productosImportados;
                    response.IsSuccess = true;
                    response.Message = "Productos importados exitosamente.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error al procesar el archivo: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseDto> GetProductsByCategoryIdAsync(int categoriaId)
        {
            var response = new ResponseDto();
            try
            {
                var productos = await _unitOfWork.Products.GetProductsByCategoryIdAsync(categoriaId);
                if (productos == null || !productos.Any())
                {
                    response.IsSuccess = false;
                    response.Message = $"No se encontraron productos para la categoría con ID {categoriaId}.";
                    return response;
                }

                response.Data = productos;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

    }
}