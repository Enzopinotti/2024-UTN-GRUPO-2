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

        // Asignar categoría a producto
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

        // Desasignar categoría de producto
        public async Task<ResponseDto> DesasignarCategoriaDeProductoAsync(int idProducto, int idCategoria)
        {
            var response = new ResponseDto();

            try
            {
                // Buscar el registro de relación de producto-categoría
                var productoCategoria = await _context.ProductoCategoria
                    .FirstOrDefaultAsync(pc => pc.idProducto == idProducto && pc.idCategoria == idCategoria);

                if (productoCategoria != null)
                {
                    // Eliminar la relación si existe
                    _context.ProductoCategoria.Remove(productoCategoria);
                    await _context.SaveChangesAsync();
                    response.IsSuccess = true; // Indicar que la operación fue exitosa
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "La relación Producto-Categoría no existe."; // Mensaje si no se encontró la relación
                }
            }
            catch (DbUpdateException ex)
            {
                // Manejo de errores específicos de la base de datos
                response.IsSuccess = false;
                response.Message = $"Error al desasignar la categoría: {ex.InnerException?.Message ?? ex.Message}"; // Mensaje de error
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                response.IsSuccess = false;
                response.Message = $"Error inesperado: {ex.Message}"; // Mensaje de error general
            }

            return response; // Retornar la respuesta
        }

        // Obtener categorías de un producto
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

        // Obtener productos de una categoría
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
