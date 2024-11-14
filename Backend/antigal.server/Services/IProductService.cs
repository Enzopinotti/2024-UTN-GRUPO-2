
//
using antigal.server.Models;
using antigal.server.Models.Dto;
using System.Collections.Generic;

namespace antigal.server.Services
{
    public interface IProductService
    {
        ResponseDto GetProducts(string orden = null, string precio = null);
        Task<ResponseDto> GetProductsHomeAsync();
        ResponseDto GetProductById(int id);
        ResponseDto GetProductByTitle(string nombre);
        ResponseDto AddProduct(Producto producto);
        ResponseDto DeleteProduct(int id);
        ResponseDto PutProduct(Producto producto);
        ResponseDto ImportProductsFromExcel(IFormFile file);
    }
}