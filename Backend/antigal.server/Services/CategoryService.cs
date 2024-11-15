// Services/CategoryService.cs
using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Repositories;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDto> GetCategoriesAsync()
        {
            return await _unitOfWork.Categories.GetCategoriesAsync();
        }

        public async Task<ResponseDto> GetCategoryByIdAsync(int id)
        {
            return await _unitOfWork.Categories.GetCategoryByIdAsync(id);
        }

        public async Task<ResponseDto> GetCategoryByTitleAsync(string nombre)
        {
            return await _unitOfWork.Categories.GetCategoriesByTitleAsync(nombre);
        }

        public async Task<ResponseDto> AddCategoryAsync(Categoria categoria)
        {
            return await _unitOfWork.Categories.AddCategoryAsync(categoria);
        }

        public async Task<ResponseDto> UpdateCategoryAsync(Categoria categoria)
        {
            return await _unitOfWork.Categories.UpdateCategoryAsync(categoria);
        }

        public async Task<ResponseDto> DeleteCategoryAsync(int id)
        {
            return await _unitOfWork.Categories.DeleteCategoryAsync(id);
        }
    }
}