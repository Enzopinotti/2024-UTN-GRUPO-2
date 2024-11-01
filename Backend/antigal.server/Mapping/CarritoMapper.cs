using antigal.server.Models.Dto.CarritoDtos;
using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Services;
using antigal.server.Repositories;
using System.Linq; // Asegúrate de incluir esto para poder usar Select

namespace antigal.server.Mapping
{
    public class CarritoMapper // Define una clase para el mapeo
    {
        public CarritoDto MapCarritoToCarritoDto(Carrito carrito)
        {
            return new CarritoDto
            {
                // Asigna las propiedades de Carrito a CarritoDto
                idCarrito = carrito.idCarrito,
                Items = carrito.Items.Select(item => new CarritoItemDto
                {
                    idProducto = item.idProducto,
                    cantidad = item.cantidad,
                    // Asigna otras propiedades si es necesario
                }).ToList()
            };
        }
    }
}
