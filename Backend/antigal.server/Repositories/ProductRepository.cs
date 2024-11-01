using antigal.server.Data;
using antigal.server.Models;
using antigal.server.Models.Dto;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace antigal.server.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ResponseDto> GetProductsAsync()
        {
            var response = new ResponseDto();
            try
            {
                IEnumerable<Producto> productos = await _context.Productos.ToListAsync();
                response.Data = productos;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> GetProductByIdAsync(int id)
        {
            var response = new ResponseDto();
            try
            {
                var producto = await _context.Productos.FirstOrDefaultAsync(p => p.idProducto == id);
                response.Data = producto;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> GetProductByTitleAsync(string nombre)
        {
            var response = new ResponseDto();
            try
            {
                var normalizedNombre = nombre.ToLower();
                var productos = await _context.Productos
                    .Where(p => p.nombre.ToLower().Contains(normalizedNombre))
                    .Select(p => new
                    {
                        p.idProducto,
                        p.nombre,
                        p.marca
                    })
                    .ToListAsync();

                response.Data = productos;
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> AddProductAsync(Producto producto)
        {
            var response = new ResponseDto();
            try
            {
                await _context.Productos.AddAsync(producto);
                await _context.SaveChangesAsync();
                response.Data = producto;
                response.IsSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> DeleteProductAsync(int id)
        {
            var response = new ResponseDto();
            try
            {
                var producto = await _context.Productos.FirstOrDefaultAsync(p => p.idProducto == id);
                if (producto == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"No se encontró el producto con ID {id}.";
                    return response;
                }

                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
                response.IsSuccess = true;
                response.Message = "Producto eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> PutProductAsync(Producto producto)
        {
            var response = new ResponseDto();
            try
            {
                _context.Productos.Update(producto);
                await _context.SaveChangesAsync();
                response.Data = producto;
                response.IsSuccess = true;
            }
            catch (DbUpdateException ex)
            {
                response.IsSuccess = false;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDto> ImportProductsFromExcelAsync(IFormFile file)
        {
            var response = new ResponseDto();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    XSSFWorkbook workbook = new XSSFWorkbook(stream);
                    var sheet = workbook.GetSheetAt(0); // Assuming first sheet contains the products

                    for (int i = 1; i <= sheet.LastRowNum; i++) // Skip header row
                    {
                        var row = sheet.GetRow(i);
                        if (row == null) continue;

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

                        int? destacado = null;
                        if (int.TryParse(destacadoCell, out int destacadoVal))
                        {
                            destacado = destacadoVal;
                        }

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

                        await _context.Productos.AddAsync(producto);
                    }

                    await _context.SaveChangesAsync();
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

        public async Task<ResponseDto> GetProductsByCategoryIdAsync(int categoriaId)
        {
            var response = new ResponseDto();
            try
            {
                // Esperar el resultado de ToListAsync primero
                var productosCategorias = await _context.ProductoCategoria
                    .Include(pc => pc.Producto)  // Load Product
                    .Include(pc => pc.Categoria) // Load Category
                    .Where(pc => pc.idCategoria == categoriaId)  // Filter by category ID
                    .ToListAsync();  // Execute the query

                // Usar Select después de haber esperado el resultado
                var productos = productosCategorias.Select(pc => new
                {
                    IdProducto = pc.Producto?.idProducto ?? 0,
                    Nombre = pc.Producto?.nombre ?? "N/A",
                    Marca = pc.Producto?.marca ?? "N/A",
                    Descripcion = pc.Producto?.descripcion ?? "N/A",
                    Precio = pc.Producto?.precio ?? 0.0f,
                    Stock = pc.Producto?.stock ?? 0,
                    Disponible = pc.Producto?.disponible ?? 0,
                    Categoria = pc.Categoria?.nombre ?? "N/A"
                }).ToList(); // No es necesario esperar de nuevo

                if (productos == null || !productos.Any())
                {
                    response.IsSuccess = false;
                    response.Message = $"No se encontraron productos para la categoría con ID {categoriaId}.";
                    return response;
                }

                response.Data = productos;
                response.IsSuccess = true;
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
