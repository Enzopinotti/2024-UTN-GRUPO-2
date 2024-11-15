// Repositories/IProductRepository.cs
using antigal.server.Models;
using antigal.server.Models.Dto;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace antigal.server.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Producto>> GetAllProductsAsync();
        Task<Producto?> GetProductByIdAsync(int id);
        Task<IEnumerable<Producto>> GetProductsByTitleAsync(string nombre);
        Task<Producto> AddProductAsync(Producto producto);
        Task<bool> UpdateProductAsync(Producto producto);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<Producto>> GetProductsByCategoryIdAsync(int categoriaId);
        Task<IEnumerable<Producto>> ImportProductsFromExcelAsync(Stream fileStream);
        Task<ResponseDto> GetProductsHomeAsync();
    }
}