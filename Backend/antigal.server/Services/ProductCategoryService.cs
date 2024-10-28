using antigal.server.Data;
using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Repositories;

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
        private ResponseDto VerificarExistencia(int idProducto, int idCategoria)
        {
            var response = new ResponseDto(); // Nueva instancia de ResponseDto

            var producto = _context.Productos.Find(idProducto);
            var categoria = _context.Categorias.Find(idCategoria);

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
        public ResponseDto AsignarCategoriaAProducto(int idProducto, int idCategoria)
        {
            var verificationResponse = VerificarExistencia(idProducto, idCategoria);

            if (!verificationResponse.IsSuccess)
            {
                return verificationResponse; // Retornar la respuesta si hubo un error
            }

            // Llamar al repositorio para asignar la categoría
            var resultResponse = _repository.AsignarCategoriaAProducto(idProducto, idCategoria);
            return resultResponse;
        }

        // Desasigna una categoría de un producto
        public ResponseDto DesasignarCategoriaDeProducto(int idProducto, int idCategoria)
        {
            var verificationResponse = VerificarExistencia(idProducto, idCategoria);

            if (!verificationResponse.IsSuccess)
            {
                return verificationResponse; // Retornar la respuesta si hubo un error
            }

            // Llamar al repositorio para desasignar la categoría
            var resultResponse = _repository.DesasignarCategoriaDeProducto(idProducto, idCategoria);
            return resultResponse;
        }

        // Obtiene las categorías de un producto
        public ResponseDto ObtenerCategoriasDeProducto(int idProducto)
        {
            return _repository.ObtenerCategoriasDeProducto(idProducto);
        }

        // Obtiene los productos de una categoría
        public ResponseDto ObtenerProductosDeCategoria(int idCategoria)
        {
            return _repository.ObtenerProductosDeCategoria(idCategoria);
        }
    }
}
