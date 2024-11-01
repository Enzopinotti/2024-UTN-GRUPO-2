using antigal.server.Data;
using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Repositories;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _repository;
        private readonly AppDbContext _context;

        public ProductCategoryService(IProductCategoryRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        // Método privado para verificar la existencia de producto y categoría
        private async Task<ResponseDto> VerificarExistenciaAsync(int idProducto, int idCategoria)
        {
            var response = new ResponseDto(); // Nueva instancia de ResponseDto

            var producto = await _context.Productos.FindAsync(idProducto);
            var categoria = await _context.Categorias.FindAsync(idCategoria);

            if (producto == null || categoria == null)
            {
                response.IsSuccess = false;
                response.Message = "Producto o Categoría no encontrados.";
            }
            else
            {
                response.IsSuccess = true; // Si se encuentran, se establece éxito
            }

            return response;
        }

        // Asigna una categoría a un producto
        public async Task<ResponseDto> AsignarCategoriaAProductoAsync(int idProducto, int idCategoria)
        {
            var verificationResponse = await VerificarExistenciaAsync(idProducto, idCategoria);

            if (!verificationResponse.IsSuccess)
            {
                return verificationResponse; // Retornar la respuesta si hubo un error
            }

            // Llamar al repositorio para asignar la categoría
            var resultResponse = await _repository.AsignarCategoriaAProductoAsync(idProducto, idCategoria);
            return resultResponse;
        }

        // Desasigna una categoría de un producto
        public async Task<ResponseDto> DesasignarCategoriaDeProductoAsync(int idProducto, int idCategoria)
        {
            var verificationResponse = await VerificarExistenciaAsync(idProducto, idCategoria);

            if (!verificationResponse.IsSuccess)
            {
                return verificationResponse; // Retornar la respuesta si hubo un error
            }

            // Llamar al repositorio para desasignar la categoría
            var resultResponse = await _repository.DesasignarCategoriaDeProductoAsync(idProducto, idCategoria);
            return resultResponse;
        }

        // Obtiene las categorías de un producto
        public async Task<ResponseDto> ObtenerCategoriasDeProductoAsync(int idProducto)
        {
            return await _repository.ObtenerCategoriasDeProductoAsync(idProducto);
        }

        // Obtiene los productos de una categoría
        public async Task<ResponseDto> ObtenerProductosDeCategoriaAsync(int idCategoria)
        {
            return await _repository.ObtenerProductosDeCategoriaAsync(idCategoria);
        }
    }
}
