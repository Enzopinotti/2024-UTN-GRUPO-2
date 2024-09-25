import React, { useContext } from 'react';
import { CartContext } from '../../contexts/CartContext';  // Importar el contexto del carrito

const CartWidget = ({ onClick }) => {
  // Acceder al contexto del carrito
  const { cartItems } = useContext(CartContext);

  // Calcular el total de productos en el carrito
  const totalItems = cartItems.reduce((total, item) => total + item.quantity, 0);

  return (
    <div className="cart-widget" onClick={onClick}>
      <img src="./images/carrito.svg" alt="carrito" width={40} className="carrito" />
      {totalItems > 0 && (
        <span className="cart-count">
          {totalItems}
        </span>
      )}
    </div>
  );
};

export default CartWidget;
