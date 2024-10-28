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
        [AllowAnonymous]
        [HttpGet("getProducts")]
        public ResponseDto GetProduct()
        {
            Console.WriteLine("funciona");
            return _productService.GetProducts();
        }
        [AllowAnonymous]
        [HttpGet("getProductById/{id}")]
        public ResponseDto GetProductById(int id)
        {
            return _productService.GetProductById(id);
        }
        [AllowAnonymous]
        [HttpGet("getProductByTitle/{nombre}")]
        public ResponseDto GetProductByTitle(string nombre)
        {
            return _productService.GetProductByTitle(nombre);
        }
    }
}