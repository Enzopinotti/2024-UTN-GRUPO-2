// Services/ProductService.cs
using antigal.server.Data;
using antigal.server.Models;
using antigal.server.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NPOI.XSSF.UserModel;
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
        public ResponseDto ImportProductsFromExcel(IFormFile file)
        {
            var response = new ResponseDto();

            try
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    stream.Position = 0;

                    // Usamos NPOI para leer el archivo Excel
                    XSSFWorkbook workbook = new XSSFWorkbook(stream);
                    var sheet = workbook.GetSheetAt(0); // Suponemos que el primer sheet contiene los productos

                    for (int i = 1; i <= sheet.LastRowNum; i++) // Saltamos la primera fila de encabezado
                    {
                        var row = sheet.GetRow(i);
                        if (row == null) continue;

                        // Validaciones de los datos
                        var nombre = row.GetCell(0)?.ToString();
                        var marca = row.GetCell(1)?.ToString();
                        var descripcion = row.GetCell(2)?.ToString();
                        var codigoBarrasCell = row.GetCell(3)?.ToString();
                        var disponibleCell = row.GetCell(4)?.ToString();
                        var destacadoCell = row.GetCell(5)?.ToString();
                        var precioCell = row.GetCell(6)?.ToString();
                        var stockCell = row.GetCell(7)?.ToString();

                        if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(marca) ||
                            !int.TryParse(codigoBarrasCell, out int codigoBarras) ||
                            !int.TryParse(disponibleCell, out int disponible) ||
                            !float.TryParse(precioCell, out float precio) ||
                            !int.TryParse(stockCell, out int stock))
                        {
                            response.IsSuccess = false;
                            response.Message = $"Error en la fila {i + 1}: datos inválidos.";
                            return response;
                        }

                        // Validamos si destacado es nulo o no
                        int? destacado = null;
                        if (int.TryParse(destacadoCell, out int destacadoVal))
                        {
                            destacado = destacadoVal;
                        }

                        // Creamos un nuevo producto basado en los datos del Excel
                        Producto producto = new Producto
                        {
                            nombre = nombre,
                            marca = marca,
                            descripcion = descripcion,
                            codigoBarras = codigoBarras,
                            disponible = disponible,
                            destacado = destacado,
                            precio = precio,
                            stock = stock
                        };

                        // Agregamos el producto a la base de datos
                        _context.Productos.Add(producto);
                    }

                    _context.SaveChanges();
                }

                response.IsSuccess = true;
                response.Message = "Productos importados exitosamente.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"Error al procesar el archivo: {ex.Message}";
            }

            return response;
        }
    }
}
