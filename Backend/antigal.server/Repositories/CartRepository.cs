using antigal.server.Data;
using antigal.server.Models;
using antigal.server.Models.Dto;
using Microsoft.EntityFrameworkCore;

namespace antigal.server.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public ResponseDto GetCartByUserId(string userId)
        {
            var cart = _context.Carritos
                .Include(c => c.Items)
                .ThenInclude(i => i.Producto)
                .FirstOrDefault(c => c.idUsuario == userId);

            if (cart != null)
            {
                return new ResponseDto { IsSuccess = true, Data = cart };
            }
            return new ResponseDto { IsSuccess = false, Message = "Carrito no encontrado." };
        }

        public ResponseDto CreateCart(string userId)
        {
            // Verificar si el usuario ya tiene un carrito
            var existingCart = _context.Carritos.FirstOrDefault(c => c.idUsuario == userId);
            if (existingCart != null)
            {
                return new ResponseDto { IsSuccess = false, Message = "El usuario ya tiene un carrito." };
            }

            // Crear un nuevo carrito
            var cart = new Carrito(userId);
            _context.Carritos.Add(cart);
            _context.SaveChanges();
            return new ResponseDto { IsSuccess = true, Data = cart };
        }


        public ResponseDto AddItemToCart(string userId, AddItemToCartDto addItemDto)
        {
            // Verificar si el producto existe
            var producto = _context.Productos.FirstOrDefault(p => p.idProducto == addItemDto.idProducto);
            if (producto == null)
            {
                return new ResponseDto { IsSuccess = false, Message = "Producto no encontrado." };
            }

            var cart = _context.Carritos.Include(c => c.Items).FirstOrDefault(c => c.idUsuario == userId);
            if (cart == null) return new ResponseDto { IsSuccess = false, Message = "Carrito no encontrado." };

            var existingItem = cart.Items.FirstOrDefault(i => i.idProducto == addItemDto.idProducto);
            if (existingItem != null)
            {
                existingItem.cantidad += addItemDto.cantidad; // Sumar cantidades si el producto ya está en el carrito
            }
            else
            {
                // Convertir el DTO en un CarritoItem antes de agregarlo al carrito
                var newItem = new CarritoItem
                {
                    idProducto = addItemDto.idProducto,
                    cantidad = addItemDto.cantidad,
                    // Puedes asignar otras propiedades aquí si es necesario
                };

                cart.Items.Add(newItem); // Agregar nuevo ítem
            }

            _context.SaveChanges();
            return new ResponseDto { IsSuccess = true, Data = cart };
        }


        public ResponseDto RemoveItemFromCart(string userId, int itemId)
        {
            var cart = _context.Carritos.Include(c => c.Items).FirstOrDefault(c => c.idUsuario == userId);
            if (cart == null) return new ResponseDto { IsSuccess = false, Message = "Carrito no encontrado." };

            var item = cart.Items.FirstOrDefault(i => i.idCarritoItem == itemId);
            if (item != null)
            {
                cart.Items.Remove(item);
                _context.SaveChanges();
                return new ResponseDto { IsSuccess = true, Data = cart };
            }

            return new ResponseDto { IsSuccess = false, Message = "Ítem no encontrado." };
        }

        public ResponseDto ClearCart(string userId)
        {
            var cart = _context.Carritos.Include(c => c.Items).FirstOrDefault(c => c.idUsuario == userId);
            if (cart == null) return new ResponseDto { IsSuccess = false, Message = "Carrito no encontrado." };

            cart.Items.Clear();
            _context.SaveChanges();
            return new ResponseDto { IsSuccess = true, Message = "Carrito vaciado correctamente." };
        }
    }
}
