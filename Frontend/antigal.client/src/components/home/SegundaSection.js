// src/components/home/SegundaSection.js
import React, { useState, useEffect } from 'react';
import OfferCard from '../common/OfferCard';
import { getVisibleItems } from '../../utils/screenUtils';
import { useSwipeable } from 'react-swipeable';

const SegundaSection = () => {
  const [windowWidth, setWindowWidth] = useState(window.innerWidth);
  const [currentIndex, setCurrentIndex] = useState(0);
  const visibleItems = getVisibleItems(windowWidth); // Productos visibles basado en el tamaño de la pantalla

  const productos = [
    {
      id: 1,
      nombre: "Granola Premium X1KG",
      marca: "GoNatural",
      precioAnterior: 3299,
      precioOferta: 2999,
      imagen: "/images/granola.png",
      estrellas: 4.5,
      totalReviews: 27,
    },
    {
      id: 2,
      nombre: "Banana Chips",
      marca: "Kerala",
      precioAnterior: 2599,
      precioOferta: 2399,
      imagen: "/images/banana_chips.png",
      estrellas: 4.7,
      totalReviews: 19,
    },
    {
      id: 3,
      nombre: "Yogurt Griego Deslactosado",
      marca: "Oikos",
      precioAnterior: 3199,
      precioOferta: 2999,
      imagen: "/images/yogurt_griego.png",
      estrellas: 4.3,
      totalReviews: 13,
    },
  
  ];

  // Manejar el redimensionamiento de la ventana
  useEffect(() => {
    const handleResize = () => {
      setWindowWidth(window.innerWidth);
    };

    window.addEventListener('resize', handleResize);
    return () => window.removeEventListener('resize', handleResize);
  }, []);

  // Funciones para avanzar y retroceder en el carrusel
  const handleNext = () => {
    setCurrentIndex((prevIndex) => (prevIndex + visibleItems) % productos.length);
  };

  const handlePrev = () => {
    setCurrentIndex((prevIndex) => {
      const newIndex = prevIndex - visibleItems;
      return newIndex < 0 ? (productos.length + newIndex) % productos.length : newIndex;
    });
  };

  // Configurar los handlers de swipe
  const handlers = useSwipeable({
    onSwipedLeft: () => handleNext(),
    onSwipedRight: () => handlePrev(),
    preventDefaultTouchmoveEvent: true,
    trackMouse: true, // Opción para soportar arrastre con mouse
  });

  // Generar los productos a mostrar, asegurando el loop
  const displayedProducts = Array.from({ length: visibleItems }, (_, i) => {
    return productos[(currentIndex + i) % productos.length];
  });

  // Calcular cuántos puntos mostrar en función de los productos visibles
  const numDots = Math.ceil(productos.length / visibleItems);

  // Manejar los puntos de paginación
  const handleDotClick = (index) => {
    setCurrentIndex(index * visibleItems);
  };

  return (
    <section className="segundaSection" style={{ backgroundImage: "url('/images/fondoCarruselOffertas.jpg')" }}>
      <h2 className="tituloOfertas">TOP PRODUCTOS RECOMENDADOS</h2>
      <div className="carousel" {...handlers}>
        {displayedProducts.map((producto) => (
          <OfferCard key={producto.id} producto={producto} />
        ))}
      </div>

      {/* Puntos de navegación */}
      <div className="pagination">
        {Array.from({ length: numDots }).map((_, index) => (
          <div
            key={index}
            className={`dot ${Math.floor(currentIndex / visibleItems) === index ? 'active' : ''}`}
            onClick={() => handleDotClick(index)}
          ></div>
        ))}
      </div>
    </section>
  );
};

export default SegundaSection;
