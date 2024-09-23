import React, { useState, useEffect, useContext } from 'react';
import { CartContext } from '../../../contexts/CartContext';  // Importar el contexto del carrito
import { useNavigate } from 'react-router-dom';
import Swal from 'sweetalert2';

const Product = ({ product }) => {
  const navigate = useNavigate();
  
  const [liked, setLiked] = useState(() => JSON.parse(localStorage.getItem(`liked-${product.id}`)) || false);
  
  const [cartCount, setCartCount] = useState(() => {
    const savedCount = JSON.parse(localStorage.getItem(`cart-${product.id}`));
    return savedCount || 0;
  });

  const { addToCart, removeFromCart } = useContext(CartContext);  // Usar el contexto del carrito

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

  // Nueva función para eliminar todos los productos de un tipo específico
  const handleRemoveFromCart = () => {
    setCartCount(0);  // Reiniciar el contador local
    removeFromCart(product.id);  // Llamar a la función del contexto para eliminar el producto
    Swal.fire({
      title: 'Producto eliminado',
      text: 'El producto ha sido eliminado del carrito',
      icon: 'info',
      confirmButtonText: 'Cerrar'
    });
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
        <h3>{product.name}</h3>
        <p className="category">Categoría: {product.category}</p>
        
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
          
          <article className='cartDeleteButton' onClick={handleRemoveFromCart}>
            <div>Eliminar del Carrito</div>
            <img src='./icons/closeCardIcon.svg' alt='Icono de cerrar de Antigal' />

          </article>
        </section>
      </div>
    </div>
  );
};

export default Product;
