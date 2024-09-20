// Services/ProductService.cs
using antigal.server.Data;
using antigal.server.Models;
using antigal.server.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace antigal.server.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private ResponseDto _response;

        public ProductService(AppDbContext context)
        {
            _context = context;
            _response = new ResponseDto();
        }

        public ResponseDto GetProducts()
        {
            try
            {
                IEnumerable<Producto> productos = _context.Productos.ToList();
                _response.Data = productos;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        public ResponseDto GetProductById(int id)
        {
            try
            {
                var producto = _context.Productos.FirstOrDefault(p => p.idProducto == id);
                _response.Data = producto;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        public ResponseDto GetProductByTitle(string nombre)
        {
            try
            {
                var normalizedNombre = nombre.ToLower();
                // Obtener productos del contexto de datos
                var productos = _context.Productos
                .Where(p => p.nombre.ToLower().Contains(normalizedNombre))
                .Select(p => new
                {
                    p.idProducto,
                    p.nombre,
                    p.marca
                })
                .ToList(); // Ejecutar la consulta

                // Asignar los productos encontrados a la respuesta
                _response.Data = productos;
                _response.IsSuccess = true; // Indicar éxito
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        public ResponseDto AddProduct(Producto producto)
        {
            try
            {
                _context.Productos.Add(producto);
                _context.SaveChanges();
                _response.Data = producto;
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
        public ResponseDto DeleteProduct(int id)
        {
            try
            {
                var producto = _context.Productos.FirstOrDefault(p => p.idProducto == id);
                _context.Remove(producto);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }
        public ResponseDto PutProduct(Producto producto)
        {
            try
            {
                _context.Productos.Update(producto);
                _context.SaveChanges();

                _response.Data = producto;
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
    }
}
