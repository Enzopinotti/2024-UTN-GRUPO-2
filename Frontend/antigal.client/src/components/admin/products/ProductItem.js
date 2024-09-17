import React from "react";

const AdminProductItem = ({ product }) => {
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
            <div><p>{product.description}</p> </div>
          
            <div className="row"> 
               <p>Precio: {product.price}</p> 
               <p>Stock: {product.stock}</p>      
             </div>
        </div>
      </div>
    </div>
  );
};

export default AdminProductItem;