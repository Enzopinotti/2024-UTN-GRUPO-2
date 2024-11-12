using antigal.server.Models.Dto;

namespace antigal.server.Services
{
    public interface IProductCategoryService
    {
        Task<ResponseDto> AsignarCategoriaAProductoAsync(int idProducto, int idCategoria);
        Task<ResponseDto> DesasignarCategoriaDeProductoAsync(int idProducto, int idCategoria);
        Task<ResponseDto> ObtenerCategoriasDeProductoAsync(int idProducto);
        Task<ResponseDto> ObtenerProductosDeCategoriaAsync(int idCategoria);
    }
}
