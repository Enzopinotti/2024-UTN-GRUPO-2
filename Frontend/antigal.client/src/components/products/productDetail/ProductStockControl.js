import React, { useState } from "react";

const ProductStockControl = ({ product }) => {
  const [quantity, setQuantity] = useState(1);
  const isOutOfStock = product.stock === 0;

  const handleIncrease = () => {
    if (quantity < product.stock) {
      setQuantity(quantity + 1);
    }
  };

  const handleDecrease = () => {
    if (quantity > 1) {
      setQuantity(quantity - 1);
    }
  };

  return (
    <div className="detail-section">
      <p className="disponibility">
        Disponibilidad: {!isOutOfStock ? `En Stock (${product.stock} disponibles)` : "Fuera de stock"}
      </p>

      <div className="cart-section">
        <div className="quantity">
          <button onClick={handleDecrease} disabled={isOutOfStock}>-</button>
          <input type="text" value={quantity} readOnly />
          <button onClick={handleIncrease} disabled={isOutOfStock}>+</button>
        </div>
        <button className={`add-to-cart ${isOutOfStock ? 'inactive' : ''}`} disabled={isOutOfStock}>Agregar al carrito</button>
      </div>
    </div>
  );
};

export default ProductStockControl;
