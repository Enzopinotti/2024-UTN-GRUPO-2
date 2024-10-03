// Services/CategoryService.cs
using antigal.server.Data;
using antigal.server.Models;
using antigal.server.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;
using System.Linq;

namespace antigal.server.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        private ResponseDto _response;

        public CategoryService(AppDbContext context)
        {
            _context = context;
            _response = new ResponseDto();
        }

        public ResponseDto GetCategories()
        {
            try
            {
                IEnumerable<Categoria> categorias = _context.Categorias.ToList();
                _response.Data = categorias;
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
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        public ResponseDto GetCategoryByTitle(string nombre)
        {
            try
            {
                var categoria = _context.Categorias.FirstOrDefault(c => c.nombre == nombre);
                _response.Data = categoria;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }
        public ResponseDto AddCategory([FromBody] Categoria categoria)
        {
            try
            {
                _context.Categorias.Add(categoria);
                _context.SaveChanges();

                _response.Data = categoria;
            }
            catch (DbUpdateException ex) // Capturar errores de la base de datos
            {
                _response.IsSuccess = false;
                _response.Message = ex.InnerException?.Message ?? ex.Message; // Capturar la excepción interna si existe
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        public ResponseDto PutCategory([FromBody] Categoria categoria)
        {
            try
            {
                _context.Categorias.Update(categoria);
                _context.SaveChanges();

                _response.Data = categoria;
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
                var categoria = _context.Categorias.FirstOrDefault(c => c.idCategoria == id);
                _context.Remove(categoria);

                _context.SaveChanges();
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
