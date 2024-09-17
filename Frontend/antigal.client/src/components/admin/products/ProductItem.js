import React, { useState } from "react";
import CategoryDropdown from "../categories/CategoryDropdown";

const AdminProductItem = ({ product }) => {
  const [showDropdown, setShowDropdown] = useState(false);
  const handleDropdownToggle = () => {
    setShowDropdown(!showDropdown);
  };
  return (
    <div className="product-item">
      <div className="product-img">
        <img src={product.imageUrl} alt={product.name} />
      </div>
      <div className="product-info">
        <div className="top-section">
          <div className="title">
            <h2>{product.name}</h2>
          </div>
          <div className="product-button">
            <button className="delete-btn">
              <i className="fa-solid fa-trash"></i> <span>Eliminar</span>
            </button>
            <button className="mod-btn">
              <i className="fa-solid fa-pencil"></i> <span>Modificar</span>
            </button>
          </div>
        </div>

        <div className="bottom-section">
          <div className="row">
            <p>{product.description}</p>{" "}
          </div>

          <div className="row">
            <div className="text">
              <p>Precio: {product.price}</p>
              <p>Stock: {product.stock}</p>
            </div>
            <div className="desplegable">
              <button onClick={handleDropdownToggle} className={showDropdown ? "active" : "inactive"}>
                Categorías
              </button>
              {showDropdown && (
                <CategoryDropdown
                  categories={[
                    "Sin Tacc",
                    "Veganos",
                    "Cereales",
                    "Suplementos",
                  ]}
                />
              )}
    
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AdminProductItem;
