using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using antigal.server.Data;
using antigal.server.Models;

namespace antigal.server.Controllers;

public static class ProductoController
{
    public static void MapProductoEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Producto").WithTags(nameof(Producto));

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Productos.ToListAsync();
        })
        .WithName("GetAllProductos")
        .WithOpenApi();

        group.MapGet("/{id}", async Task<Results<Ok<Producto>, NotFound>> (int idproducto, AppDbContext db) =>
        {
            return await db.Productos.AsNoTracking()
                .FirstOrDefaultAsync(model => model.idProducto == idproducto)
                is Producto model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetProductoById")
        .WithOpenApi();

        group.MapPut("/{id}", async Task<Results<Ok, NotFound>> (int idproducto, Producto producto, AppDbContext db) =>
        {
            var affected = await db.Productos
                .Where(model => model.idProducto == idproducto)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.idProducto, producto.idProducto)
                    .SetProperty(m => m.nombre, producto.nombre)
                    .SetProperty(m => m.marca, producto.marca)
                    .SetProperty(m => m.descripcion, producto.descripcion)
                    .SetProperty(m => m.codigoBarras, producto.codigoBarras)
                    .SetProperty(m => m.disponible, producto.disponible)
                    .SetProperty(m => m.destacado, producto.destacado)
                    .SetProperty(m => m.precio, producto.precio)
                    .SetProperty(m => m.stock, producto.stock)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateProducto")
        .WithOpenApi();

        group.MapPost("/", async (Producto producto, AppDbContext db) =>
        {
            db.Productos.Add(producto);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Producto/{producto.idProducto}", producto);
        })
        .WithName("CreateProducto")
        .WithOpenApi();

        group.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (int idproducto, AppDbContext db) =>
        {
            var affected = await db.Productos
                .Where(model => model.idProducto == idproducto)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteProducto")
        .WithOpenApi();

        group.MapGet("/buscar/{termino}", async Task<List<Producto>> (string termino, AppDbContext db) =>
        {
            return await db.Productos
                .Where(p => p.nombre.ToLower().Contains(termino.ToLower()) || (p.descripcion != null && p.descripcion.Contains(termino, StringComparison.CurrentCultureIgnoreCase)))
                .ToListAsync();
        })
         .WithName("BuscarProductosPorNombreODescripcion")
         .WithOpenApi();
    }
}

