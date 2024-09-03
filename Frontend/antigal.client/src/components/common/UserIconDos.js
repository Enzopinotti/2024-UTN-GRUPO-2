// src/components/common/UserIconDos.js
import React, { useState } from 'react';

const UserIconDos = () => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [isAnimating, setIsAnimating] = useState(false);
  const isAuthenticated = false; // Aquí manejarás la lógica de autenticación

  const toggleMenu = () => {
    if (isMenuOpen) {
      setIsAnimating(true); // Inicia la animación de cierre
      setTimeout(() => {
        setIsAnimating(false);
        setIsMenuOpen(false); // Cierra el menú después de la animación
      },60); // Duración de la anima  ción debe coincidir
    } else {
      setIsMenuOpen(true);
    }
  };

  return (
    <article className='sessionContainer'>
      {(isMenuOpen || isAnimating) && (
        <ul className={`user-menu ${isAnimating ? 'close' : 'open'}`}>
          {isAuthenticated ? (
            <>
              <li>Perfil</li>
              <li>Cerrar Sesión</li>
            </>
          ) : (
            <>
              <li>Registrarse</li>
              <li>Iniciar Sesión</li>
            </>
          )}
        </ul>
      )}
      <div className="user-icon-container">
        <img 
          src="/icons/userIcon.svg" 
          alt="user icon de Antigal" 
          onClick={toggleMenu} 
          className="user-icon" 
        />
      </div>
    </article>
  );
};

export default UserIconDos;
