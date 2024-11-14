
//
using antigal.server.Models;
using antigal.server.Models.Dto;
using System.Collections.Generic;

namespace antigal.server.Services
{
    public interface IProductService
    {
        ResponseDto GetProducts();
        ResponseDto GetProductById(int id);
        ResponseDto GetProductByTitle(string nombre);
        ResponseDto AddProduct(Producto producto);
        ResponseDto DeleteProduct(int id);
        ResponseDto PutProduct(Producto producto);
        ResponseDto ImportProductsFromExcel(IFormFile file);
    }
}