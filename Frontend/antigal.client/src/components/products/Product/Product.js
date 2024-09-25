import React, { useState, useEffect, useContext } from 'react';
import { CartContext } from '../../../contexts/CartContext';  // Importar el contexto del carrito
import { useNavigate } from 'react-router-dom';
import Swal from 'sweetalert2';
import formatCamelCase from '../../../utils/formatCamelCase';  // Importamos la función para formatear

const Product = ({ product }) => {
  const navigate = useNavigate();
  
  const [liked, setLiked] = useState(() => JSON.parse(localStorage.getItem(`liked-${product.id}`)) || false);
  
  const [cartCount, setCartCount] = useState(() => {
    const savedCount = JSON.parse(localStorage.getItem(`cart-${product.id}`));
    return savedCount || 0;
  });

  const { addToCart } = useContext(CartContext);  // Usar el contexto del carrito

  useEffect(() => {
    localStorage.setItem(`liked-${product.id}`, JSON.stringify(liked));
  }, [liked, product.id]);

  useEffect(() => {
    localStorage.setItem(`cart-${product.id}`, JSON.stringify(cartCount));
  }, [cartCount, product.id]);

  const handleImageClick = () => {
    navigate(`/products/${product.id}`);
  };

  const handleAddToCart = () => {
    const newCount = cartCount + 1;
    setCartCount(newCount);
    addToCart(product, 1); 

    Swal.fire({
      title: '¡Excelente!',
      text: 'Producto añadido al carrito correctamente',
      icon: 'success',
      confirmButtonText: 'Cerrar'
    });
  };

  const handleLike = () => {
    setLiked(!liked);
  };


  return (
    <div className="product-item">
      {product.onSale && <div className="sale-tag">SALE</div>}
      <article className="imageAndIcons">
        <img src={product.imageUrl} alt={product.name} onClick={handleImageClick} />
        <div className="actions">
          {/* Botón de like */}
          <button 
            className="like-button" 
            onClick={handleLike}
            key={liked ? 'liked' : 'not-liked'}  
          >
            <img 
              src={liked ? './icons/likeRellenoIcon.png' : './icons/likeIcon.png'} 
              alt="like icon"
            /> 
          </button>
        </div>
      </article>
      
      <div className="info">
        {/* Aplicamos la función formatCamelCase al nombre del producto */}
        <h3>{formatCamelCase(product.name)}</h3>
        
        {/* Aplicamos la función formatCamelCase a la categoría */}
        <p className="category">{formatCamelCase(product.category)}</p>
        
        <p className="offerPrice">
          {product.onSale && (
            <span className="precioAnterior">
              ${parseFloat(product.price).toFixed(2)}
            </span>
          )}
          <span className="precioOferta">
            ${product.onSale ? parseFloat(product.salePrice).toFixed(2) : parseFloat(product.price).toFixed(2)}
          </span>
        </p>
      </div>
      <div>
        <section className='buttonsContainer'>
          <article className='cartButton' onClick={handleAddToCart}>
            <div>Agregar al Carrito</div>
            <img src='./icons/cartCardIcon.svg' alt='Icono de carrito de Antigal' />
          </article>
        </section>
      </div>
    </div>
  );
};

export default Product;
