// src/components/products/productDetail/ProductDetail.js
import React from 'react';
import { useParams } from 'react-router-dom';

const ProductDetail = () => {
  const { id } = useParams();

  // Aquí puedes hacer un fetch al backend para obtener los detalles del producto usando el id

  return (
    <div>
      <h2>Detalle del Producto</h2>
      <p>Producto ID: {id}</p>
      {/* Aquí mostrarás los detalles reales del producto */}
    </div>
  );
};

export default ProductDetail;