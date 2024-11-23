import React from "react";
import { useNavigate } from "react-router-dom";

const CheckoutPage = () => {
  const navigate = useNavigate();

  const handlePlaceOrder = () => {
    // Acción simulada al realizar el pedido
    alert("Pedido realizado con éxito. ¡Gracias por tu compra!");
    navigate("/"); // Redirige al usuario a la página principal
  };

  return (
    <div className="checkout-page">
      <h1 className="checkout-title">Finalizar Compra</h1>
      <div className="checkout-container">
        {/* Información del cliente */}
        <div className="checkout-section">
          <h2>Información del Cliente</h2>
          <form className="checkout-form">
            <label>
              Nombre:
              <input type="text" placeholder="Tu nombre" required />
            </label>
            <label>
              Correo Electrónico:
              <input type="email" placeholder="Tu correo" required />
            </label>
            <label>
              Teléfono:
              <input type="tel" placeholder="Tu número de teléfono" required />
            </label>
          </form>
        </div>

        {/* Detalles del envío */}
        <div className="checkout-section">
          <h2>Dirección de Envío</h2>
          <form className="checkout-form">
            <label>
              Dirección:
              <input type="text" placeholder="Calle y número" required />
            </label>
            <label>
              Ciudad:
              <input type="text" placeholder="Ciudad" required />
            </label>
            <label>
              Código Postal:
              <input type="text" placeholder="Código postal" required />
            </label>
            <label>
              Provincia:
              <input type="text" placeholder="Provincia" required />
            </label>
          </form>
        </div>

        {/* Resumen del pedido */}
        <div className="checkout-section">
          <h2>Resumen del Pedido</h2>
          <div className="order-summary">
            <div className="summary-item">
              <span>Subtotal:</span>
              <span>$2000</span>
            </div>
            <div className="summary-item">
              <span>Envío:</span>
              <span>$10</span>
            </div>
            <div className="summary-item">
              <span>Impuestos:</span>
              <span>$200</span>
            </div>
            <hr />
            <div className="summary-item total">
              <span>Total:</span>
              <span>$2210</span>
            </div>
          </div>
        </div>

        {/* Botón para finalizar la compra */}
        <div className="checkout-footer">
          <button
            className="place-order-button"
            onClick={handlePlaceOrder}
          >
            Realizar Pedido
          </button>
        </div>
      </div>
    </div>
  );
};

export default CheckoutPage;
