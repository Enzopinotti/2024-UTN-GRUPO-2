import React from "react";
import '@fortawesome/fontawesome-free/css/all.min.css';


const ProductMoreInfo = () => {
  return (
    <div className="info-section">
      <div className="more-info">
        <p>
          <i className="fa-solid fa-credit-card"></i> Medios de Pago
        </p>
        <button>+</button>
      </div>
      <div className="more-info">
        <p>
          <i className="fa-solid fa-truck"></i> Medios de Envio
        </p>
        <button>+</button>
      </div>
      <div className="more-info">
        <p>
          <i className="fa-solid fa-house"></i> Retiro en el local
        </p>
        <button>+</button>
      </div>
    </div>
  );
};

export default ProductMoreInfo;
