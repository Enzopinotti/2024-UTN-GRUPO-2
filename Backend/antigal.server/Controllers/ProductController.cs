// Controllers/ProductController.cs
using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace antigal.server.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpPost("addProduct")]
        public Task<ResponseDto> PostProduct([FromBody] Producto producto)
        {
            return _productService.AddProductAsync(producto);
        }

        [HttpPut("updateProduct")]
        public Task<ResponseDto> PutProductAsync([FromBody] Producto producto)
        {
            return _productService.PutProductAsync(producto);
        }

        [HttpDelete("deleteProduct/{id}")]
        public Task<ResponseDto> DeleteProductAsync(int id)
        {
            return _productService.DeleteProductAsync(id);
        }

        [HttpPost("uploadProductsFromExcel")]
        public Task<ResponseDto> ImportProductsFromExcelAsync(IFormFile file)
        {
            return _productService.ImportProductsFromExcelAsync(file);
        }
        [AllowAnonymous]
        [HttpGet("getProducts")]
        public Task<ResponseDto> GetProductsAsync()
        {
            Console.WriteLine("funciona");
            return _productService.GetProductsAsync();
        }
        [AllowAnonymous]
        [HttpGet("getProductById/{id}")]
        public Task<ResponseDto> GetProductByIdAsync(int id)
        {
            return _productService.GetProductByIdAsync(id);
        }
        [AllowAnonymous]
        [HttpGet("getProductByTitle/{nombre}")]
        public Task<ResponseDto> GetProductByTitleAsync(string nombre)
        {
            return _productService.GetProductByTitleAsync(nombre);
        }

        [AllowAnonymous]
        [HttpGet("home")]
        public Task<ResponseDto> GetHome()
        {
            return _productService.GetProductsHomeAsync();
        }



    }
}