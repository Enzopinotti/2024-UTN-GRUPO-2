// src/components/products/productList/ProductListContainer.js
import React, { useState, useEffect } from 'react';
import ProductList from './ProductList';

const ProductListContainer = () => {
  const [products, setProducts] = useState([]);

  useEffect(() => {
    // Array de productos de ejemplo
    const productData = [
      {
        id: 1,
        name: 'Manzana',
        price: 1.2,
        imageUrl: '/images/manzana.jpg',
        category: 'Frutas',
      },
      {
        id: 2,
        name: 'Lechuga',
        price: 0.8,
        imageUrl: '/images/lechuga.jpg',
        category: 'Verduras',
      },
      {
        id: 3,
        name: 'Avena',
        price: 2.5,
        imageUrl: '/images/avena.jpg',
        category: 'Cereales',
      },
      // Agrega más productos según sea necesario
    ];

    setProducts(productData);

    // Código preparado para realizar fetch a la API del backend (comentado por ahora)
    /*
    fetch('http://localhost:5000/api/products')
      .then((response) => response.json())
      .then((data) => setProducts(data))
      .catch((error) => console.error('Error fetching products:', error));
    */
  }, []);

  return <ProductList products={products} />;
};

export default ProductListContainer;
