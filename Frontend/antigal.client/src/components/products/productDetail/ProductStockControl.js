import React, { useState } from "react";
import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

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
  const handleAddToCart =()=>{
    const cart = JSON.parse(localStorage.getItem("cart"))||[];
    const existingProductIndex = cart.findIndex((item)=>item.id === product.id);

    if (existingProductIndex!==-1){
      cart[existingProductIndex].quantity+=quantity;
    }else{
      cart.push({
        id:product.id,
        name:product.name,
        price: product.price,
        quantity:quantity,

      });
    }
    localStorage.setItem("cart",JSON.stringify(cart));
    toast.success(`${product.name} ha sido agregado al carrito (${quantity})`,{
      className: 'toast',
      position:'top-right',
      autoClose: 1000,
      hideProgressBar: true,
      draggable: true,
      progress: undefined,
      closeOnClick: true,
      pauseOnHover: true,
    });

  }
  return (
    <div className="detail-section">
      <p className="disponibility">
        Disponibilidad:{" "}
        {!isOutOfStock
          ? `En Stock (${product.stock} disponibles)`
          : "Fuera de stock"}
      </p>

      <div className="cart-section">
        <div className="quantity">
          <button onClick={handleDecrease} disabled={isOutOfStock}>
            -
          </button>
          <input type="text" value={quantity} readOnly />
          <button onClick={handleIncrease} disabled={isOutOfStock}>
            +
          </button>
        </div>
        <button
          className={`add-to-cart ${isOutOfStock ? "inactive" : ""}`}
          disabled={isOutOfStock}
          onClick={handleAddToCart}
        >
          Agregar al carrito
        </button>
      </div>
    </div>
  );
};

export default ProductStockControl;
