using antigal.server.Models;
using antigal.server.Models.Dto.VentaDtos;
using antigal.server.Services;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class SaleController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SaleController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpPost("realizar")]
    public async Task<IActionResult> RealizarVenta([FromBody] SaleRequestDto request)
    {
        try
        {
            var result = await _saleService.CreateSaleAsync(request.idUsuario, request.idOrden, request.total, request.metodoPago);

            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error inesperado", Details = ex.Message });
        }
    }

    [HttpGet("{idVenta}")]
    public async Task<IActionResult> GetSaleById(int idVenta)
    {
        try
        {
            var sale = await _saleService.GetSaleByIdAsync(idVenta);

            if (sale == null)
            {
                return NotFound(new { Message = "Venta no encontrada" });
            }

            return Ok(sale);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error inesperado", Details = ex.Message });
        }
    }

    [HttpPut("{idVenta}/estado")]
    public async Task<IActionResult> UpdateSaleStatus(int idVenta, [FromBody] string nuevoEstado)
    {
        try
        {
            // Intentamos convertir el string en un enum de VentaEstado
            if (Enum.TryParse<VentaEstado>(nuevoEstado, true, out var estado))
            {
                var result = await _saleService.UpdateSaleStatusAsync(idVenta, estado); // Usamos el enum en el servicio
                if (result)
                {
                    return Ok(new { Message = "Estado de la venta actualizado correctamente" });
                }
                return NotFound(new { Message = "Venta no encontrada" });
            }
            else
            {
                // Si el string no es un valor válido de VentaEstado
                return BadRequest(new { Message = "Estado de venta no válido" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error inesperado", Details = ex.Message });
        }
    }



}
