// src/components/products/product/Product.js
import React from 'react';
import { useNavigate } from 'react-router-dom';

const Product = ({ product }) => {
  const navigate = useNavigate();

  const handleClick = () => {
    navigate(`/products/${product.id}`);
  };

  return (
    <div className="product-item" onClick={handleClick} style={{ cursor: 'pointer' }}>
      <img src={product.imageUrl} alt={product.name} />
      <h3>{product.name}</h3>
      <p>Categoría: {product.category}</p>
      <p>Precio: ${product.price}</p>
    </div>
  );
};

export default Product;
