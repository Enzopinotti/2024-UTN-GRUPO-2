// Services/IProductService.cs
using antigal.server.Models;
using antigal.server.Models.Dto;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public interface IProductService
    {
        Task<ResponseDto> GetProducts(string orden = null, string precio = null);
        Task<ResponseDto> GetProductByIdAsync(int id);
        Task<ResponseDto> GetProductByTitleAsync(string nombre);
        Task<ResponseDto> AddProductAsync(Producto producto);
        Task<ResponseDto> DeleteProductAsync(int id);
        Task<ResponseDto> PutProductAsync(Producto producto);
        Task<ResponseDto> ImportProductsFromExcelAsync(IFormFile file);
        Task<ResponseDto> GetProductsByCategoryIdAsync(int categoriaId);
        Task<ResponseDto> GetProductsHomeAsync();
    }
}