// src/components/common/UserIconDos.js
import React, { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth0 } from '@auth0/auth0-react';

const UserIconDos = () => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const { isAuthenticated, loginWithRedirect, logout, user } = useAuth0();
  const navigate = useNavigate();
  const menuRef = useRef(null);

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
  };

  // Manejar clics fuera del menú
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (menuRef.current && !menuRef.current.contains(event.target)) {
        setIsMenuOpen(false);
      }
    };
    if (isMenuOpen) {
      document.addEventListener('mousedown', handleClickOutside);
    }
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [isMenuOpen]);

  // Funciones de navegación
  const goToProfile = () => {
    navigate('/profile');
    setIsMenuOpen(false);
  };

  return (
    <article className='sessionContainer' ref={menuRef}>
      {isMenuOpen && (
        <ul className='user-menu'>
          {isAuthenticated ? (
            <>
              <li onClick={goToProfile}>Perfil</li>
              <li onClick={() => logout({ returnTo: window.location.origin })}>Cerrar Sesión</li>
            </>
          ) : (
            <>
              <li onClick={() => loginWithRedirect({ screen_hint: 'signup' })}>Registrarse</li>
              <li onClick={() => loginWithRedirect()}>Iniciar Sesión</li>
            </>
          )}
        </ul>
      )}
      <div className="user-icon-container">
        <img
          src="/icons/userIcon.svg"
          alt="user icon"
          onClick={toggleMenu}
          className="user-icon"
        />
      </div>
    </article>
  );
};

export default UserIconDos;