// Repositories/ICategoriaRepository.cs
using antigal.server.Models;
using antigal.server.Models.Dto;
using System.Threading.Tasks;

namespace antigal.server.Repositories
{
    public interface ICategoriaRepository
    {
        Task<ResponseDto> GetCategoriesAsync();
        Task<ResponseDto> GetCategoryByIdAsync(int id);
        Task<ResponseDto> GetCategoriesByTitleAsync(string nombre);
        Task<ResponseDto> AddCategoryAsync(Categoria categoria);
        Task<ResponseDto> UpdateCategoryAsync(Categoria categoria);
        Task<ResponseDto> DeleteCategoryAsync(int id);
    }
}