// Repositories/ProductRepository.cs
using antigal.server.Data;
using antigal.server.Models;
using antigal.server.Models.Dto;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace antigal.server.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> GetProductsAsync(string orden, string precio)
        {
            var productos = _context.Productos.AsQueryable();

            // Aplicar ordenamiento por fecha
            if (orden == "antiguos")
            {
                productos = productos.OrderBy(p => p.FechaCreacion);
            }
            else if (orden == "recientes")
            {
                productos = productos.OrderByDescending(p => p.FechaCreacion);
            }

            // Aplicar ordenamiento por precio
            if (precio == "ascendente")
            {
                productos = productos.OrderBy(p => p.precio);
            }
            else if (precio == "descendente")
            {
                productos = productos.OrderByDescending(p => p.precio);
            }

            return await productos.ToListAsync();
        }

        public async Task<Producto?> GetProductByIdAsync(int id)
        {
            return await _context.Productos.FirstOrDefaultAsync(p => p.idProducto == id);
        }

        public async Task<IEnumerable<Producto>> GetProductsByTitleAsync(string nombre)
        {
            var normalizedNombre = nombre.ToLower();
            return await _context.Productos
                .Where(p => p.nombre.ToLower().Contains(normalizedNombre))
                .ToListAsync();
        }

        public async Task<Producto> AddProductAsync(Producto producto)
        {
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<bool> UpdateProductAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.idProducto == id);
            if (producto == null)
                return false;

            _context.Productos.Remove(producto);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<IEnumerable<Producto>> GetProductsByCategoryIdAsync(int categoriaId)
        {
            var productosCategorias = await _context.ProductoCategoria
                .Include(pc => pc.Producto)
                .Include(pc => pc.Categoria)
                .Where(pc => pc.idCategoria == categoriaId)
                .ToListAsync();

            var productos = productosCategorias
                .Select(pc => pc.Producto)
                .Where(p => p != null)
                .Cast<Producto>();

            return productos;
        }

        public async Task<IEnumerable<Producto>> ImportProductsFromExcelAsync(Stream fileStream)
        {
            var productosImportados = new List<Producto>();

            try
            {
                using (var workbook = new NPOI.XSSF.UserModel.XSSFWorkbook(fileStream))
                {
                    var sheet = workbook.GetSheetAt(0); // Suponiendo que la primera hoja contiene los productos

                    for (int i = 1; i <= sheet.LastRowNum; i++) // Saltar la fila de encabezado
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
                            !decimal.TryParse(precioCell, out decimal precio) ||
                            !int.TryParse(stockCell, out int stock))
                        {
                            // Manejar errores de formato según sea necesario
                            continue;
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
                        productosImportados.Add(producto);
                    }

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }

            return productosImportados;
        }

        public async Task<List<Producto>> GetFeaturedProductsAsync()
        {
            return await _context.Productos
                .Where(p => p.destacado == 1)
                .ToListAsync();
        }
    }
}