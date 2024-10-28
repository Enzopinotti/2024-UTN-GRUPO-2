using antigal.server.Models;
using antigal.server.Models.Dto;

namespace antigal.server.Repositories
{
    public interface ICategoriaRepository
    {
        ResponseDto GetCategories(); // Devolviendo ResponseDto
        ResponseDto GetCategoryById(int id); // Devolviendo ResponseDto
        ResponseDto GetCategoriesByTitle(string nombre); // Devolviendo ResponseDto
        ResponseDto AddCategory(Categoria categoria); // Devolviendo ResponseDto
        ResponseDto UpdateCategory(Categoria categoria); // Devolviendo ResponseDto
        ResponseDto DeleteCategory(int id); // Devolviendo ResponseDto
    }
}
