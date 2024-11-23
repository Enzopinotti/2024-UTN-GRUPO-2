import React, { useState } from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';

const OfferCard = ({ producto, isDesktop, reverse }) => {
  const {
    idProducto, // Asegúrate de que el id del producto esté presente
    nombre,
    marca,
    precioAnterior,
    precioOferta,
    imagen,
    estrellas,
    totalReviews,
    descripcion
  } = producto;

  // Estado para manejar los likes
  const [likes, setLikes] = useState(0);

  // Función para incrementar los likes
  const handleLike = () => {
    setLikes(likes + 1);
  };

  return (
<<<<<<< HEAD
    <div className={`offerCard ${isDesktop ? (reverse ? 'row-reverse' : 'row') : ''}`}>
      {/* Imagen del producto */}
      <img src={imagen} alt={`Imagen de ${nombre}`} className="offerImage" />
=======
    <Link to={`/products/${idProducto}`} className="offerCardLink">
      <div className={`offerCard ${isDesktop ? (reverse ? 'row-reverse' : 'row') : ''}`}>
        <img src={imagen} alt={`Imagen de ${nombre}`} className="offerImage" />
>>>>>>> FrontEnd

        {/* Detalles del producto */}
        <div className="offerDetails">
          <h3 className="offerName">{nombre}</h3>
          <p className="offerBrand">{marca}</p>
          <p className="offerDescription">{descripcion}</p>

<<<<<<< HEAD
        {/* Precios */}
        <p className="offerPrice">
          <span className="precioAnterior">${precioAnterior}</span>
          <span className="precioOferta">${precioOferta}</span>
        </p>
=======
          {/* Precios */}
          <p className="offerPrice">
            <span className="precio">${precio}</span>
            <span className="precioOferta">${precioOferta}</span>
          </p>
>>>>>>> FrontEnd

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
<<<<<<< HEAD
    precioAnterior: PropTypes.number.isRequired,
    precioOferta: PropTypes.number.isRequired,
    imagen: PropTypes.string.isRequired,
    estrellas: PropTypes.number.isRequired,
    totalReviews: PropTypes.number.isRequired,
=======
    precio: PropTypes.number.isRequired,
    precioOferta: PropTypes.number.isRequired, // Agregado para evitar problemas de validación
>>>>>>> FrontEnd
    descripcion: PropTypes.string.isRequired,
  }).isRequired,
  isDesktop: PropTypes.bool.isRequired,
  reverse: PropTypes.bool.isRequired,
};

export default OfferCard;
