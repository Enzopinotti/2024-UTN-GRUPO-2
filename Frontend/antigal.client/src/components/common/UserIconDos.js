import React, { useState } from 'react';
import Swal from 'sweetalert2'; // Importamos SweetAlert2

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
      }, 60); // Duración de la animación debe coincidir
    } else {
      setIsMenuOpen(true);
    }
  };

  // Función para mostrar el mensaje de "Funcionalidad en Desarrollo"
  const showDevelopmentAlert = () => {
    Swal.fire({
      title: 'Funcionalidad en Desarrollo',
      text: 'Esta funcionalidad estará disponible pronto.',
      icon: 'info',
      confirmButtonText: 'Cerrar'
    });
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
              {/* Alerta de desarrollo para "Registrarse" */}
              <li onClick={showDevelopmentAlert}>Registrarse</li>

              {/* Alerta de desarrollo para "Iniciar Sesión" */}
              <li onClick={showDevelopmentAlert}>Iniciar Sesión</li>
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
