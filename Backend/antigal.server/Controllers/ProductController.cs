// Controllers/ProductController.cs
using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Relationships;
using antigal.server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using antigal.server.Data;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly AppDbContext _context; // Añadido
        private readonly IImageService _imageService; // Añadido


        public ProductController(IProductService productService, AppDbContext context, IImageService imageService)
        {
            _productService = productService;
            _context = context; // Inicializado
            _imageService = imageService; // Inicializado
        }

        [HttpGet("getProducts")]
        public ResponseDto GetProduct()
        {
            Console.WriteLine("funciona");
            return _productService.GetProducts();
        }

        [HttpGet("getProductById/{id}")]
        public ResponseDto GetProductById(int id)
        {
            return _productService.GetProductById(id);
        }

        [HttpGet("getProductByTitle/{nombre}")]
        public ResponseDto GetProductByTitle(string nombre)
        {
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