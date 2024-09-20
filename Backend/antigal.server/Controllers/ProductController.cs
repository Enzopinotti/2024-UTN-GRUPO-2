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

        [HttpGet("GetProduct")]
        public ResponseDto GetProduct()
        {
            return _productService.GetProducts();
        }

        [HttpGet("GetProductById/{id}")]
        public ResponseDto GetProductById(int id)
        {
            return _productService.GetProductById(id);
        }

        [HttpGet("GetProductByTitle/{nombre}")]
        public ResponseDto GetProductByTitle(string nombre)
        {
            return _productService.GetProductByTitle(nombre);
        }

        [HttpPost("PostProduct")]
        public ResponseDto PostProduct([FromBody] Producto producto)
        {
            return _productService.AddProduct(producto);
        }

        [HttpDelete("DeleteProduct/{int id}")]
        public ResponseDto DeleteProduct(int id)
        {
            return _productService.DeleteProduct(id);
        }

        [HttpPut("PutProduct")]
        public ResponseDto PutProduct([FromBody] Producto producto)
        {
            return _productService.PutProduct(producto);
        }
    }
}