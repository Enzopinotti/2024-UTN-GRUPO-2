using antigal.server.Models;
using antigal.server.Models.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public interface IProductService
    {
        Task<ResponseDto> GetProductsAsync();
        Task<ResponseDto> GetProductByIdAsync(int id);
        Task<ResponseDto> GetProductByTitleAsync(string nombre);
        Task<ResponseDto> AddProductAsync(Producto producto);
        Task<ResponseDto> DeleteProductAsync(int id);
        Task<ResponseDto> PutProductAsync(Producto producto);
        Task<ResponseDto> ImportProductsFromExcelAsync(IFormFile file);
        Task<ResponseDto> GetProductsByCategoryIdAsync(int categoriaId); // Este método falta en ProductService
    }
}
