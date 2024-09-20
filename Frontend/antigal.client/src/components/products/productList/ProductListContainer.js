import React, { useState, useEffect } from 'react';
import ProductList from './ProductList';
import { adaptProductsForSale } from '../../../utils/onSaleMock';

const ProductListContainer = () => {
  const [products, setProducts] = useState([]);

  useEffect(() => {
    const useBackend = false; // Cambia este flag a true para conectar con el backend

    const fetchURL = useBackend ? 'http://localhost:5000/api/products' : 'https://fakestoreapi.com/products';

    fetch(fetchURL)
      .then(response => response.json())
      .then(data => {
        // Mapea y adapta los productos para incluir lógica de oferta
        const adaptedData = adaptProductsForSale(data.map(item => ({
          id: item.id,
          name: item.title,
          price: item.price,
          imageUrl: item.image,
          category: item.category,
        })));
        setProducts(adaptedData); // Actualiza el estado con los productos adaptados
      })
      .catch(error => console.error('Error fetching products:', error));
  }, []);

  return <ProductList products={products} />;
};

export default ProductListContainer;
