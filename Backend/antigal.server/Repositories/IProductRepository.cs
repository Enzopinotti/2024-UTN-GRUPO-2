// Repositories/IProductRepository.cs
using antigal.server.Models;
using antigal.server.Models.Dto;
using NPOI.SS.Formula.Functions;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace antigal.server.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Producto>> GetProductsAsync(string orden, string precio);
        Task<Producto?> GetProductByIdAsync(int id);
        Task<IEnumerable<Producto>> GetProductsByTitleAsync(string nombre);
        Task<Producto> AddProductAsync(Producto producto);
        Task<bool> UpdateProductAsync(Producto producto);
        Task<bool> DeleteProductAsync(int id);
        Task<IEnumerable<Producto>> GetProductsByCategoryIdAsync(int categoriaId);
        Task<IEnumerable<Producto>> ImportProductsFromExcelAsync(Stream fileStream);
        Task<List<Producto>> GetFeaturedProductsAsync();
    }
}