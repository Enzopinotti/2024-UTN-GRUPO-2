// src/components/common/UserIconDos.js
import React, { useState } from 'react';

const UserIconDos = () => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const isAuthenticated = false; // Aquí manejarás la lógica de autenticación

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
  };

  return (
    <div className="user-icon-container">
      {isMenuOpen && (
        <ul className="user-menu">
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
      <img 
        src="/icons/userIcon.png" 
        alt="user icon de Antigal" 
        onClick={toggleMenu} 
        className="user-icon" 
      />
      
    </div>
  );
};

export default UserIconDos;
