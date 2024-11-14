using antigal.server.Data;
using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Relationships;
using Azure;
using Microsoft.EntityFrameworkCore;

namespace antigal.server.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly AppDbContext _context;
        private ResponseDto _response;

        public ProductCategoryRepository(AppDbContext context)
        {
            _context = context;
            _response = new ResponseDto();
        }

        // Asignar categoría a producto
        public ResponseDto AsignarCategoriaAProducto(int idProducto, int idCategoria)
        {
            try
            {
                var productoCategoria = new ProductoCategoria
                {
                    idProducto = idProducto,
                    idCategoria = idCategoria
                };

                _context.ProductoCategoria.Add(productoCategoria);
                _context.SaveChanges();

                _response.IsSuccess = true;
                _response.Data = productoCategoria;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"Error al asignar categoría: {ex.Message}";
            }

            return _response;
        }


        // Desasignar categoría de producto
        public ResponseDto DesasignarCategoriaDeProducto(int idProducto, int idCategoria)
        {
            try
            {
                // Buscar el registro de relación de producto-categoría
                var productoCategoria = _context.ProductoCategoria
                    .FirstOrDefault(pc => pc.idProducto == idProducto && pc.idCategoria == idCategoria);

                if (productoCategoria != null)
                {
                    // Eliminar la relación si existe
                    _context.ProductoCategoria.Remove(productoCategoria);
                    _context.SaveChanges();
                    _response.IsSuccess = true; // Indicar que la operación fue exitosa
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = "La relación Producto-Categoría no existe."; // Mensaje si no se encontró la relación
                }
            }
            catch (DbUpdateException ex)
            {
                // Manejo de errores específicos de la base de datos
                _response.IsSuccess = false;
                _response.Message = $"Error al desasignar la categoría: {ex.InnerException?.Message ?? ex.Message}"; // Mensaje de error
            }
            catch (Exception ex)
            {
                // Manejo de errores generales
                _response.IsSuccess = false;
                _response.Message = $"Error inesperado: {ex.Message}"; // Mensaje de error general
            }

            return _response; // Retornar la respuesta
        }


        // Obtener categorías de un producto
        public ResponseDto ObtenerCategoriasDeProducto(int idProducto)
        {
            try
            {
                var categorias = _context.ProductoCategoria
                    .Where(pc => pc.idProducto == idProducto)
                    .Include(pc => pc.Categoria)
                    .Select(pc => pc.Categoria)
                    .ToList();

                _response.IsSuccess = true;
                _response.Data = categorias;
            }
            catch (DbUpdateException ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"Error al obtener categorías: {ex.InnerException?.Message ?? ex.Message}";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"Error inesperado: {ex.Message}";
            }

            return _response;
        }


        // Obtener productos de una categoría
        public ResponseDto ObtenerProductosDeCategoria(int idCategoria)
        {
            try
            {
                var productos = _context.ProductoCategoria
                    .Where(pc => pc.idCategoria == idCategoria)
                    .Include(pc => pc.Producto)
                    .Select(pc => pc.Producto)
                    .ToList();

                _response.IsSuccess = true;
                _response.Data = productos;
            }
            catch (DbUpdateException ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"Error al obtener productos: {ex.InnerException?.Message ?? ex.Message}";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = $"Error inesperado: {ex.Message}";
            }
            return _response;
        }

    }
}
