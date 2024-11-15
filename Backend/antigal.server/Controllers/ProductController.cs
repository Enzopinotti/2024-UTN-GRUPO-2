// Controllers/ProductController.cs
using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Services;
using Microsoft.AspNetCore.Mvc;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("getProducts")]
        public ResponseDto GetProduct(string orden = null, string precio = null)
        {
            Console.WriteLine("funciona");
            return _productService.GetProducts(orden, precio);
        }

        [HttpGet("home")]
        public Task<ResponseDto> GetHome()
        {
            return _productService.GetProductsHomeAsync();
        }

        [HttpGet("getProductById/{id}")]
        public ResponseDto GetProductById(int id)
        {
            return _productService.GetProductById(id);
        }

        [HttpGet("getProductByTitle/{nombre}")]
        public ResponseDto GetProductByTitle(string nombre)
        {
            Console.WriteLine("El nombre es: " + nombre);
            return _productService.GetProductByTitle(nombre);
        }

        [HttpPost("addProduct")]
        public ResponseDto PostProduct([FromBody] Producto producto)
        {
            return _productService.AddProduct(producto);
        }

        [HttpPut("updateProduct")]
        public ResponseDto PutProduct([FromBody] Producto producto)
        {
            return _productService.PutProduct(producto);
        }

        [HttpDelete("deleteProduct/{id}")]
        public ResponseDto DeleteProduct(int id)
        {
            return _productService.DeleteProduct(id);
        }

        [HttpPost("uploadProductsFromExcel")]
        public ResponseDto UploadProductsFromExcel(IFormFile file)
        {
            return _productService.ImportProductsFromExcel(file);
        }

    }
}