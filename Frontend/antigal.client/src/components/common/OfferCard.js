// src/components/common/OfferCard.js
import React, { useState } from 'react';
import PropTypes from 'prop-types';

const OfferCard = ({ producto }) => {
  const {
    nombre,
    marca,
    precioAnterior,
    precioOferta,
    imagen,
    estrellas,
    totalReviews,
  } = producto;

  // Estado para manejar los likes
  const [likes, setLikes] = useState(0);

  // Función para incrementar los likes
  const handleLike = () => {
    setLikes(likes + 1);
  };

  return (
    <div className="offerCard">
      {/* Imagen del producto */}
      <img src={imagen} alt={`Imagen de ${nombre}`} className="offerImage" />

      {/* Detalles del producto */}
      <h3 className="offerName">{nombre}</h3>
      <p className="offerBrand">{marca}</p>

      {/* Precios */}
      <p className="offerPrice">
        <span className="precioAnterior">${precioAnterior}</span>
        <span className="precioOferta">${precioOferta}</span>
      </p>

      {/* Rating */}
      <div className="offerRating">
        {Array(Math.round(estrellas)).fill('⭐')}
        <span>({totalReviews})</span>
      </div>

      {/* Botón Comprar */}
      <button className="comprarButton">Comprar</button>

      {/* Botón de Like */}
      <div className="likeSection">
        <button className="likeButton" onClick={handleLike}>
          👍 {likes} {likes === 1 ? 'Like' : 'Likes'}
        </button>
      </div>
    </div>
  );
};

OfferCard.propTypes = {
  producto: PropTypes.shape({
    nombre: PropTypes.string.isRequired,
    marca: PropTypes.string.isRequired,
    precioAnterior: PropTypes.number.isRequired,
    precioOferta: PropTypes.number.isRequired,
    imagen: PropTypes.string.isRequired,
    estrellas: PropTypes.number.isRequired,
    totalReviews: PropTypes.number.isRequired,
  }).isRequired,
};

export default OfferCard;
