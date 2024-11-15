// Repositories/CategoriaRepository.cs
using antigal.server.Data;
using antigal.server.Models;
using antigal.server.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace antigal.server.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly AppDbContext _context;

        public CategoriaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseDto> GetCategoriesAsync()
        {
            var response = new ResponseDto();
            try
            {
                var categorias = await _context.Categorias.ToListAsync();
                response.Data = categorias;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ResponseDto> GetCategoryByIdAsync(int id)
        {
            var response = new ResponseDto();
            try
            {
                var categoria = await _context.Categorias.FindAsync(id);
                response.Data = categoria;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ResponseDto> GetCategoriesByTitleAsync(string nombre)
        {
            var response = new ResponseDto();
            try
            {
                var categorias = await _context.Categorias
                    .Where(c => c.nombre.ToLower().Contains(nombre.ToLower()))
                    .ToListAsync();
                response.Data = categorias;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ResponseDto> AddCategoryAsync(Categoria categoria)
        {
            var response = new ResponseDto();
            try
            {
                await _context.Categorias.AddAsync(categoria);
                await _context.SaveChangesAsync();
                response.Data = categoria;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ResponseDto> UpdateCategoryAsync(Categoria categoria)
        {
            var response = new ResponseDto();
            try
            {
                _context.Categorias.Update(categoria);
                await _context.SaveChangesAsync();
                response.Data = categoria;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ResponseDto> DeleteCategoryAsync(int id)
        {
            var response = new ResponseDto();
            try
            {
                var categoria = await _context.Categorias.FindAsync(id);
                if (categoria != null)
                {
                    _context.Categorias.Remove(categoria);
                    await _context.SaveChangesAsync();
                    response.IsSuccess = true;
                    response.Message = "Categoría eliminada exitosamente.";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = $"No se encontró la categoría con ID {id}.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}