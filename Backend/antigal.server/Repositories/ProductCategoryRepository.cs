// Repositories/ProductCategoryRepository.cs
using antigal.server.Data;
using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Relationships;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace antigal.server.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly AppDbContext _context;

        public ProductCategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseDto> AsignarCategoriaAProductoAsync(int idProducto, int idCategoria)
        {
            var response = new ResponseDto();

            try
            {
                var productoCategoria = new ProductoCategoria
                {
                    idProducto = idProducto,
                    idCategoria = idCategoria
                };

                await _context.ProductoCategoria.AddAsync(productoCategoria);
                await _context.SaveChangesAsync();

                response.IsSuccess = true;
                response.Data = productoCategoria;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error al asignar categoría: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseDto> DesasignarCategoriaDeProductoAsync(int idProducto, int idCategoria)
        {
            var response = new ResponseDto();

            try
            {
                var productoCategoria = await _context.ProductoCategoria
                    .FirstOrDefaultAsync(pc => pc.idProducto == idProducto && pc.idCategoria == idCategoria);

                if (productoCategoria != null)
                {
                    _context.ProductoCategoria.Remove(productoCategoria);
                    await _context.SaveChangesAsync();
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "La relación Producto-Categoría no existe.";
                }
            }
            catch (DbUpdateException ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error al desasignar la categoría: {ex.InnerException?.Message ?? ex.Message}";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error inesperado: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseDto> ObtenerCategoriasDeProductoAsync(int idProducto)
        {
            var response = new ResponseDto();

            try
            {
                var categorias = await _context.ProductoCategoria
                    .Where(pc => pc.idProducto == idProducto)
                    .Include(pc => pc.Categoria)
                    .Select(pc => pc.Categoria)
                    .ToListAsync();

                response.IsSuccess = true;
                response.Data = categorias;
            }
            catch (DbUpdateException ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error al obtener categorías: {ex.InnerException?.Message ?? ex.Message}";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error inesperado: {ex.Message}";
            }

            return response;
        }

        public async Task<ResponseDto> ObtenerProductosDeCategoriaAsync(int idCategoria)
        {
            var response = new ResponseDto();

            try
            {
                var productos = await _context.ProductoCategoria
                    .Where(pc => pc.idCategoria == idCategoria)
                    .Include(pc => pc.Producto)
                    .Select(pc => pc.Producto)
                    .ToListAsync();

                response.IsSuccess = true;
                response.Data = productos;
            }
            catch (DbUpdateException ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error al obtener productos: {ex.InnerException?.Message ?? ex.Message}";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error inesperado: {ex.Message}";
            }

            return response;
        }
    }
}