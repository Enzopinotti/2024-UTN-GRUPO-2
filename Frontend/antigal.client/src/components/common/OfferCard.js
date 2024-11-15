import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';

const OfferCard = ({ producto, isDesktop, reverse }) => {
  const {
    idProducto, // Asegúrate de que el id del producto esté presente
    nombre,
    marca,
    precio,
    precioOferta,
    imagenUrls,
    descripcion,
    estrellas = 0, // Valor por defecto si no se proporciona
    totalReviews = 0 // Valor por defecto si no se proporciona
  } = producto;

  const imagen = imagenUrls?.$values[0] || '';

  // Estado para manejar los likes
  const [likes, setLikes] = useState(0);

  // Función para incrementar los likes
  const handleLike = () => {
    setLikes(likes + 1);
  };

  return (
    <Link to={`/products/${idProducto}`} className="offerCardLink">
      <div className={`offerCard ${isDesktop ? (reverse ? 'row-reverse' : 'row') : ''}`}>
        <img src={imagen} alt={`Imagen de ${nombre}`} className="offerImage" />

        {/* Detalles del producto */}
        <div className="offerDetails">
          <h3 className="offerName">{nombre}</h3>
          <p className="offerBrand">{marca}</p>
          <p className="offerDescription">{descripcion}</p>

          {/* Precios */}
          <p className="offerPrice">
            <span className="precio">${precio}</span>
            <span className="precioOferta">${precioOferta}</span>
          </p>

          {/* Rating */}
          <div className="offerRating">
            {Array(Math.round(estrellas))
              .fill('⭐')
              .map((star, index) => (
                <span key={index}>{star}</span>
              ))}
            <span>({totalReviews})</span>
          </div>

          {/* Botón de Like */}
          <div className="likeSection">
            <button className="likeButton" onClick={(e) => {
              e.preventDefault(); // Evita que el enlace se active al dar like
              handleLike();
            }}>
              👍 {likes} {likes === 1 ? 'Like' : 'Likes'}
            </button>
          </div>
        </div>
      </div>
    </Link>
  );
};

OfferCard.propTypes = {
  producto: PropTypes.shape({
    idProducto: PropTypes.number.isRequired, // ID necesario para el enlace
    nombre: PropTypes.string.isRequired,
    marca: PropTypes.string.isRequired,
    precio: PropTypes.number.isRequired,
    precioOferta: PropTypes.number.isRequired, // Agregado para evitar problemas de validación
    descripcion: PropTypes.string.isRequired,
    imagenUrls: PropTypes.shape({
      $values: PropTypes.arrayOf(PropTypes.string).isRequired
    }).isRequired,
    estrellas: PropTypes.number, // Este campo puede ser opcional
    totalReviews: PropTypes.number // Este campo puede ser opcional
  }).isRequired,
  isDesktop: PropTypes.bool.isRequired,
  reverse: PropTypes.bool.isRequired,
};

export default OfferCard;
