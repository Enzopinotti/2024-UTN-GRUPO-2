using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Repositories;

namespace antigal.server.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoryService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public ResponseDto GetCategories()
        {
            return _categoriaRepository.GetCategories();
        }

        public ResponseDto GetCategoryById(int id)
        {
            return _categoriaRepository.GetCategoryById(id);
        }

        public ResponseDto GetCategoryByTitle(string nombre)
        {
            return _categoriaRepository.GetCategoriesByTitle(nombre);
        }

        public ResponseDto AddCategory(Categoria categoria)
        {
            return _categoriaRepository.AddCategory(categoria);
        }

        public ResponseDto UpdateCategory(Categoria categoria)
        {
            return _categoriaRepository.UpdateCategory(categoria);
        }

        public ResponseDto DeleteCategory(int id)
        {
            return _categoriaRepository.DeleteCategory(id);
        }
    }
}
