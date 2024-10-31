using antigal.server.Models;
using antigal.server.Models.Dto;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public interface ICategoryService
    {
        Task<ResponseDto> GetCategoriesAsync();
        Task<ResponseDto> GetCategoryByIdAsync(int id);
        Task<ResponseDto> GetCategoryByTitleAsync(string nombre);
        Task<ResponseDto> AddCategoryAsync(Categoria categoria);
        Task<ResponseDto> UpdateCategoryAsync(Categoria categoria);
        Task<ResponseDto> DeleteCategoryAsync(int id);
    }
}
