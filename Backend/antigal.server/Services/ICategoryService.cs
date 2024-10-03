using antigal.server.Models;
using antigal.server.Models.Dto;

namespace antigal.server.Services
{
    public interface ICategoryService
    {
        ResponseDto GetCategories();
        ResponseDto GetCategoryById(int id);
        ResponseDto GetCategoryByTitle(string nombre);
        ResponseDto AddCategory(Categoria categoria);
        ResponseDto UpdateCategory(Categoria categoria);
        ResponseDto DeleteCategory(int id);
    }
}
