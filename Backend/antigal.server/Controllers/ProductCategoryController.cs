using antigal.server.Models.Dto; // Asegúrate de incluir el espacio de nombres correcto para ResponseDto
using antigal.server.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks; // Importar para Task

namespace antigal.server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryService _productCategoryService;

        public ProductCategoryController(IProductCategoryService productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        [HttpPost("asignar")]
        public async Task<ActionResult<ResponseDto>> AsignarCategoriaAProductoAsync(int idProducto, int idCategoria)
        {
            var response = await _productCategoryService.AsignarCategoriaAProductoAsync(idProducto, idCategoria);

            if (!response.IsSuccess)
            {
                // Retorna un código 400 (Bad Request) si la operación falla con ResponseDto
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Message = response.Message
                });
            }

            // Retorna 201 (Created) cuando se asigna correctamente con ResponseDto
            return CreatedAtAction(nameof(ObtenerCategoriasDeProductoAsync), new { idProducto }, response);
        }

        [HttpDelete("desasignar")]
        public async Task<ActionResult<ResponseDto>> DesasignarCategoriaDeProductoAsync(int idProducto, int idCategoria)
        {
            var response = await _productCategoryService.DesasignarCategoriaDeProductoAsync(idProducto, idCategoria);

            if (!response.IsSuccess)
            {
                // Retorna un código 404 (Not Found) si la relación no existe
                return NotFound(new ResponseDto
                {
                    IsSuccess = false,
                    Message = response.Message
                });
            }

            // Retorna 204 (No Content) si la operación se realiza correctamente
            return NoContent();
        }

        [HttpGet("categorias/{idProducto}")]
        public async Task<ActionResult<ResponseDto>> ObtenerCategoriasDeProductoAsync(int idProducto)
        {
            var response = await _productCategoryService.ObtenerCategoriasDeProductoAsync(idProducto);

            if (!response.IsSuccess)
            {
                // Retorna un código 404 (Not Found) si el producto no tiene categorías
                return NotFound(new ResponseDto
                {
                    IsSuccess = false,
                    Message = response.Message
                });
            }

            // Retorna 200 (OK) cuando se obtienen las categorías
            return Ok(new ResponseDto
            {
                IsSuccess = true,
                Data = response.Data
            });
        }

        [HttpGet("productos/{idCategoria}")]
        public async Task<ActionResult<ResponseDto>> ObtenerProductosDeCategoriaAsync(int idCategoria)
        {
            var response = await _productCategoryService.ObtenerProductosDeCategoriaAsync(idCategoria);

            if (!response.IsSuccess)
            {
                // Retorna un código 404 (Not Found) si la categoría no tiene productos
                return NotFound(new ResponseDto
                {
                    IsSuccess = false,
                    Message = response.Message
                });
            }

            // Retorna 200 (OK) cuando se obtienen los productos
            return Ok(new ResponseDto
            {
                IsSuccess = true,
                Data = response.Data
            });
        }
    }
}
