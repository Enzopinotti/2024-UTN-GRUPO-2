using antigal.server.Data;
using antigal.server.Models;
using antigal.server.Models.Dto;

namespace antigal.server.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly AppDbContext _context;
        private readonly ResponseDto _response;

        public CategoriaRepository(AppDbContext context)
        {
            _context = context;
            _response = new ResponseDto();
        }

        public ResponseDto GetCategories()
        {
            try
            {
                var categorias = _context.Categorias.ToList();
                _response.Data = categorias;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public ResponseDto GetCategoryById(int id)
        {
            try
            {
                var categoria = _context.Categorias.FirstOrDefault(c => c.idCategoria == id);
                _response.Data = categoria;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public ResponseDto GetCategoriesByTitle(string nombre)
        {
            try
            {
                var categorias = _context.Categorias
                    .Where(c => c.nombre.ToLower().Contains(nombre.ToLower()))
                    .ToList();
                _response.Data = categorias;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public ResponseDto AddCategory(Categoria categoria)
        {
            try
            {
                _context.Categorias.Add(categoria);
                _context.SaveChanges();
                _response.Data = categoria;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public ResponseDto UpdateCategory(Categoria categoria)
        {
            try
            {
                _context.Categorias.Update(categoria);
                _context.SaveChanges();
                _response.Data = categoria;
                _response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public ResponseDto DeleteCategory(int id)
        {
            try
            {
                var categoria = _context.Categorias.Find(id);
                if (categoria != null)
                {
                    _context.Categorias.Remove(categoria);
                    _context.SaveChanges();
                    _response.IsSuccess = true;
                    _response.Message = "Categoría eliminada exitosamente.";
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Message = $"No se encontró la categoría con ID {id}.";
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
