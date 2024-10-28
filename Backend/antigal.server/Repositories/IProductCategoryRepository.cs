using antigal.server.Models.Dto;

namespace antigal.server.Repositories
{
    public interface IProductCategoryRepository
    {
        ResponseDto AsignarCategoriaAProducto(int idProducto, int idCategoria);
        ResponseDto DesasignarCategoriaDeProducto(int idProducto, int idCategoria);
        ResponseDto ObtenerCategoriasDeProducto(int idProducto);
        ResponseDto ObtenerProductosDeCategoria(int idCategoria);
    }
}
