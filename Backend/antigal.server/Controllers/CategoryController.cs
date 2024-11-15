using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace antigal.server.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("addCategory")]
        public async Task<ResponseDto> AddCategory([FromBody] Categoria categoria)
        {
            return await _categoryService.AddCategoryAsync(categoria);
        }

        [HttpPut("updateCategory")]
        public async Task<ResponseDto> UpdateCategory([FromBody] Categoria categoria)
        {
            return await _categoryService.UpdateCategoryAsync(categoria);
        }

        [HttpDelete("deleteCategory/{id}")]
        public async Task<ResponseDto> DeleteCategory(int id)
        {
            return await _categoryService.DeleteCategoryAsync(id);
        }

        [AllowAnonymous]
        [HttpGet("getCategories")]
        public async Task<ResponseDto> GetCategories()
        {
            return await _categoryService.GetCategoriesAsync();
        }

        [AllowAnonymous]
        [HttpGet("getCategoryById/{id}")]
        public async Task<ResponseDto> GetCategoryById(int id)
        {
            return await _categoryService.GetCategoryByIdAsync(id);
        }

        [AllowAnonymous]
        [HttpGet("getCategoryByTitle/{nombre}")]
        public async Task<ResponseDto> GetCategoryByTitle(string nombre)
        {
            return await _categoryService.GetCategoryByTitleAsync(nombre);
        }
    }
}
