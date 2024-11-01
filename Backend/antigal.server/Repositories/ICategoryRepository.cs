using antigal.server.Models;
using antigal.server.Models.Dto;
using System.Threading.Tasks;

namespace antigal.server.Repositories
{
    public interface ICategoriaRepository
    {
        Task<ResponseDto> GetCategoriesAsync(); // Devolviendo ResponseDto de forma asíncrona
        Task<ResponseDto> GetCategoryByIdAsync(int id); // Devolviendo ResponseDto de forma asíncrona
        Task<ResponseDto> GetCategoriesByTitleAsync(string nombre); // Devolviendo ResponseDto de forma asíncrona
        Task<ResponseDto> AddCategoryAsync(Categoria categoria); // Devolviendo ResponseDto de forma asíncrona
        Task<ResponseDto> UpdateCategoryAsync(Categoria categoria); // Devolviendo ResponseDto de forma asíncrona
        Task<ResponseDto> DeleteCategoryAsync(int id); // Devolviendo ResponseDto de forma asíncrona
    }
}
