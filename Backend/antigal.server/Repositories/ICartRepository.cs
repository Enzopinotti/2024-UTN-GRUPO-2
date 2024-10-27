﻿using antigal.server.Models;
using antigal.server.Models.Dto;

namespace antigal.server.Repositories
{
    public interface ICartRepository
    {
        ResponseDto GetCartByUserId(string userId);  // Obtener el carrito por UserId
        ResponseDto CreateCart(string userId);  // Crear un carrito vacío para el usuario
        ResponseDto AddItemToCart(string userId, AddItemToCartDto addItemDto);  // Agregar un ítem al carrito
        ResponseDto RemoveItemFromCart(string userId, int itemId);  // Eliminar un ítem del carrito
        ResponseDto ClearCart(string userId);  // Vaciar el carrito
    }
}