using antigal.server.Data;
using antigal.server.Models;
using antigal.server.Models.Dto;
using antigal.server.Models.Dto.CarritoDtos;
using antigal.server.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace antigal.server.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;
        private readonly CarritoMapper _carritoMapper; // Instancia del mapeador


        public CartRepository(AppDbContext context, CarritoMapper carritoMapper)
        {
            _context = context;
            _carritoMapper = carritoMapper;

        }

        public async Task<ResponseDto> GetCartByUserIdAsync(string userId)
        {
            var cart = await _context.Carritos
                .Include(c => c.Items)
                .ThenInclude(i => i.Producto)
                .FirstOrDefaultAsync(c => c.idUsuario == userId);

            if (cart != null)
            {
                // Mapea el Carrito a CarritoDto
                var carritoDto = _carritoMapper.MapCarritoToCarritoDto(cart);
                return new ResponseDto { IsSuccess = true, Data = carritoDto };
            }
            return new ResponseDto { IsSuccess = false, Message = "Carrito no encontrado." };
        }


        public async Task<ResponseDto> CreateCartAsync(string userId)
        {
            var existingCart = await _context.Carritos.FirstOrDefaultAsync(c => c.idUsuario == userId);
            if (existingCart != null)
            {
                return new ResponseDto { IsSuccess = false, Message = "El usuario ya tiene un carrito." };
            }

            var cart = new Carrito(userId);
            await _context.Carritos.AddAsync(cart);
            await _context.SaveChangesAsync();
            return new ResponseDto { IsSuccess = true, Data = cart };
        }

        public async Task<ResponseDto> AddItemToCartAsync(string userId, CarritoItemDto addItemDto)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.idProducto == addItemDto.idProducto);
            if (producto == null)
            {
                return new ResponseDto { IsSuccess = false, Message = "Producto no encontrado." };
            }

            var cart = await _context.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.idUsuario == userId);
            if (cart == null) return new ResponseDto { IsSuccess = false, Message = "Carrito no encontrado." };

            var existingItem = cart.Items.FirstOrDefault(i => i.idProducto == addItemDto.idProducto);
            if (existingItem != null)
            {
                existingItem.cantidad += addItemDto.cantidad;
            }
            else
            {
                var newItem = new CarritoItem
                {
                    idProducto = addItemDto.idProducto,
                    cantidad = addItemDto.cantidad,
                };

                cart.Items.Add(newItem);
            }

            await _context.SaveChangesAsync();
            return new ResponseDto { IsSuccess = true, Data = cart };
        }

        public async Task<ResponseDto> RemoveItemFromCartAsync(string userId, int itemId)
        {
            var cart = await _context.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.idUsuario == userId);
            if (cart == null) return new ResponseDto { IsSuccess = false, Message = "Carrito no encontrado." };

            var item = cart.Items.FirstOrDefault(i => i.idCarritoItem == itemId);
            if (item != null)
            {
                cart.Items.Remove(item);
                await _context.SaveChangesAsync();
                return new ResponseDto { IsSuccess = true, Data = cart };
            }

            return new ResponseDto { IsSuccess = false, Message = "Ítem no encontrado." };
        }

        public async Task<ResponseDto> ClearCartAsync(string userId)
        {
            var cart = await _context.Carritos.Include(c => c.Items).FirstOrDefaultAsync(c => c.idUsuario == userId);
            if (cart == null) return new ResponseDto { IsSuccess = false, Message = "Carrito no encontrado." };

            cart.Items.Clear();
            await _context.SaveChangesAsync();
            return new ResponseDto { IsSuccess = true, Message = "Carrito vaciado correctamente." };
        }
    }
}
