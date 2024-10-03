
//
using antigal.server.Models;
using antigal.server.Models.Dto;
using System.Collections.Generic;

namespace antigal.server.Services
{
    public interface ICategoryService
    {
        ResponseDto GetCategories();
        ResponseDto GetCategoryById(int id);
        ResponseDto GetCategoryByTitle(string nombre);
        ResponseDto AddCategory(Categoria categoria);
        ResponseDto DeleteCategory(int id);
        ResponseDto PutCategory(Categoria categoria);
    }
}