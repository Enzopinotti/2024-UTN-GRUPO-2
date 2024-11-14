using antigal.server.Models.Dto;

namespace antigal.server.Services
{
    public interface IProductCategoryService
    {
        ResponseDto AsignarCategoriaAProducto(int idProducto, int idCategoria);
        ResponseDto DesasignarCategoriaDeProducto(int idProducto, int idCategoria);
        ResponseDto ObtenerCategoriasDeProducto(int idProducto);
        ResponseDto ObtenerProductosDeCategoria(int idCategoria);
    }
}
