using antigal.server.Services;
using Microsoft.AspNetCore.Mvc;
using antigal.server.Models.Dto; // Asegúrate de incluir el espacio de nombres correcto para ResponseDto

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
        public ActionResult<ResponseDto> AsignarCategoriaAProducto(int idProducto, int idCategoria)
        {
            var response = _productCategoryService.AsignarCategoriaAProducto(idProducto, idCategoria);

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
            return CreatedAtAction(nameof(ObtenerCategoriasDeProducto), new { idProducto }, response);
        }

        [HttpDelete("desasignar")]
        public ActionResult<ResponseDto> DesasignarCategoriaDeProducto(int idProducto, int idCategoria)
        {
            var response = _productCategoryService.DesasignarCategoriaDeProducto(idProducto, idCategoria);

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
        public ActionResult<ResponseDto> ObtenerCategoriasDeProducto(int idProducto)
        {
            var response = _productCategoryService.ObtenerCategoriasDeProducto(idProducto);

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
        public ActionResult<ResponseDto> ObtenerProductosDeCategoria(int idCategoria)
        {
            var response = _productCategoryService.ObtenerProductosDeCategoria(idCategoria);

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
