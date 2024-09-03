import React from 'react';
import { useNavigate } from 'react-router-dom';

const CategoryCarousel = () => {
  const navigate = useNavigate();

  // Ejemplo de categorías; en una aplicación real, podrías obtener estas categorías de una API
  const categories = [
    { id: 1, name: 'Frutas', imageUrl: '/images/frutas.jpg' },
    { id: 2, name: 'Verduras', imageUrl: '/images/verduras.jpg' },
    { id: 3, name: 'Cereales', imageUrl: '/images/cereales.jpg' },
    // Agrega más categorías según sea necesario
  ];

  const handleCategoryClick = (categoryName) => {
    // Redirige a la página de productos con la categoría seleccionada
    navigate(`/products?category=${categoryName}`);
  };

  return (
    <div className="category-carousel">
      <h2>Categorías</h2>
      <div className="carousel-container">
        {categories.map((category) => (
          <div
            key={category.id}
            className="carousel-item"
            onClick={() => handleCategoryClick(category.name)}
          >
            <img src={category.imageUrl} alt={category.name} />
            <h3>{category.name}</h3>
          </div>
        ))}
      </div>
    </div>
  );
};

export default CategoryCarousel;
