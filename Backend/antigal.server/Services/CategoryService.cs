using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Repositories;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoryService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<ResponseDto> GetCategoriesAsync()
        {
            return await _categoriaRepository.GetCategoriesAsync();
        }

        public async Task<ResponseDto> GetCategoryByIdAsync(int id)
        {
            return await _categoriaRepository.GetCategoryByIdAsync(id);
        }

        public async Task<ResponseDto> GetCategoryByTitleAsync(string nombre)
        {
            return await _categoriaRepository.GetCategoriesByTitleAsync(nombre);
        }

        public async Task<ResponseDto> AddCategoryAsync(Categoria categoria)
        {
            return await _categoriaRepository.AddCategoryAsync(categoria);
        }

        public async Task<ResponseDto> UpdateCategoryAsync(Categoria categoria)
        {
            return await _categoriaRepository.UpdateCategoryAsync(categoria);
        }

        public async Task<ResponseDto> DeleteCategoryAsync(int id)
        {
            return await _categoriaRepository.DeleteCategoryAsync(id);
        }
    }
}
