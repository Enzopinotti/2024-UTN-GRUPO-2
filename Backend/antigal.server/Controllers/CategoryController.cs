﻿using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Services;
using Microsoft.AspNetCore.Mvc;

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("getCategories")]
        public ResponseDto GetCategories()
        {
            return _categoryService.GetCategories();
        }

        [HttpGet("getCategoryById/{id}")]
        public ResponseDto GetCategoryById(int id)
        {
            return _categoryService.GetCategoryById(id);
        }

        [HttpGet("getCategoryByTitle/{nombre}")]
        public ResponseDto GetCategoryByTitle(string nombre)
        {
            return _categoryService.GetCategoryByTitle(nombre);
        }

        [HttpPost("addCategory")]
        public ResponseDto AddCategory([FromBody] Categoria categoria)
        {
            return _categoryService.AddCategory(categoria);
        }

        [HttpPut("updateCategory")]
        public ResponseDto UpdateCategory([FromBody] Categoria categoria)
        {
            return _categoryService.UpdateCategory(categoria);
        }

        [HttpDelete("deleteCategory/{id}")]
        public ResponseDto DeleteCategory(int id)
        {
            return _categoryService.DeleteCategory(id);
        }
    }
}
