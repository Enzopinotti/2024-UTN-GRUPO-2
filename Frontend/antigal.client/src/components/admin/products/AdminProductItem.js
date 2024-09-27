// src/components/admin/products/ProductItem.js
import React, { useState } from 'react';

const AdminProductItem = ({ product, onEdit, onDelete }) => {
  const {
    idProducto,
    nombre,
    precio,
    imagen,
    descripcion,
    marca,
    stock,
    disponible,
    codigoBarras,
  } = product;

  const [imageError, setImageError] = useState(false);

  const handleImageError = () => {
    setImageError(true);
  };

  return (
    <div className="product-item">
      <div className="product-img">
        {imagen && !imageError ? (
          <img src={imagen} alt={nombre} onError={handleImageError} />
        ) : (
          <div className="no-image">Sin Imagen</div>
        )}
      </div>
      <div className="product-info">
        <div className="top-section">
          <h2>{nombre}</h2>
          <p>{descripcion}</p>
        </div>
        <div className="bottom-section">
          <p>Precio: ${precio}</p>
          <p>Stock: {stock}</p>
          <p>Marca: {marca}</p>
          <p>Código de Barras: {codigoBarras}</p>
          <p>Disponible: {disponible ? 'Sí' : 'No'}</p>
        </div>
        <div className="product-button">
          <button className="mod-btn" onClick={() => onEdit(product)}>
            Modificar
          </button>
          <button className="delete-btn" onClick={() => onDelete(idProducto)}>
            Eliminar
          </button>
        </div>
      </div>
    </div>
  );
};

export default AdminProductItem;
