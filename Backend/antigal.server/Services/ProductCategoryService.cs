// Services/ProductCategoryService.cs
using antigal.server.Models.Dto;
using antigal.server.Repositories;
using antigal.server.Data;
using antigal.server.Models;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private async Task<ResponseDto> VerificarExistenciaAsync(int idProducto, int idCategoria)
        {
            var response = new ResponseDto();

            var producto = await _unitOfWork.Products.GetProductByIdAsync(idProducto);
            var categoria = await _unitOfWork.Categories.GetCategoryByIdAsync(idCategoria);

            if (producto == null || categoria == null)
            {
                response.IsSuccess = false;
                response.Message = "Producto o Categoría no encontrados.";
            }
            else
            {
                response.IsSuccess = true;
            }

            return response;
        }

        public async Task<ResponseDto> AsignarCategoriaAProductoAsync(int idProducto, int idCategoria)
        {
            var verificationResponse = await VerificarExistenciaAsync(idProducto, idCategoria);

            if (!verificationResponse.IsSuccess)
            {
                return verificationResponse;
            }

            var resultResponse = await _unitOfWork.ProductCategories.AsignarCategoriaAProductoAsync(idProducto, idCategoria);
            return resultResponse;
        }

        public async Task<ResponseDto> DesasignarCategoriaDeProductoAsync(int idProducto, int idCategoria)
        {
            var verificationResponse = await VerificarExistenciaAsync(idProducto, idCategoria);

            if (!verificationResponse.IsSuccess)
            {
                return verificationResponse;
            }

            var resultResponse = await _unitOfWork.ProductCategories.DesasignarCategoriaDeProductoAsync(idProducto, idCategoria);
            return resultResponse;
        }

        public async Task<ResponseDto> ObtenerCategoriasDeProductoAsync(int idProducto)
        {
            return await _unitOfWork.ProductCategories.ObtenerCategoriasDeProductoAsync(idProducto);
        }

        public async Task<ResponseDto> ObtenerProductosDeCategoriaAsync(int idCategoria)
        {
            return await _unitOfWork.ProductCategories.ObtenerProductosDeCategoriaAsync(idCategoria);
        }
    }
}