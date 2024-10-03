using Microsoft.AspNetCore.Mvc;
using antigal.server.Models;
using antigal.server.Data;
using antigal.server.Models.Dto;
using antigal.server.Services;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("getCategories")]
        public ResponseDto GetCategory()
        {
            return _categoryService.GetCategories();
        }

        [HttpGet("getCategoryById/{id}")]
        public ResponseDto GetCategoryById(int id)
        {
            return _categoryService.GetCategoryById(id);
        }

        [HttpGet("getProductByTitle/{nombre}")]
        public ResponseDto GetProductByTitle(string nombre)
        {
            return _categoryService.GetCategoryByTitle(nombre);
        }

        [HttpPost("addCategory")]
        public ResponseDto PostCategory([FromBody] Categoria categoria)
        {
            return _categoryService.AddCategory(categoria);
        }

        [HttpPut("updateCategory")]
        public ResponseDto PutCategory([FromBody] Categoria categoria)
        {
            return _categoryService.PutCategory(categoria);
        }

        [HttpDelete("deleteCategory/{id}")]
        public ResponseDto DeleteProduct(int id)
        {
            return _categoryService.DeleteCategory(id);
        }
    }
}