// src/components/common/UserIconDos.js
import React, { useState, useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import Swal from 'sweetalert2';

const UserIconDos = () => {
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const isAuthenticated = true; // Suponiendo que esta variable controla si el usuario está autenticado como admin
  const navigate = useNavigate();
  const menuRef = useRef(null);

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
  };

  // Función para manejar clics fuera del menú
  const handleClickOutside = (event) => {
    if (menuRef.current && !menuRef.current.contains(event.target)) {
      setIsMenuOpen(false);
    }
  };

  // useEffect para agregar y limpiar el event listener
  useEffect(() => {
    if (isMenuOpen) {
      document.addEventListener('mousedown', handleClickOutside);
    } else {
      document.removeEventListener('mousedown', handleClickOutside);
    }
    // Limpieza al desmontar el componente
    return () => {
      document.removeEventListener('mousedown', handleClickOutside);
    };
  }, [isMenuOpen]);

  // Navegar al Dashboard de Admin
  const goToAdminDashboard = () => {
    navigate('/admin/products');
    setIsMenuOpen(false); // Cerrar el menú después de navegar
  };

  return (
    <article className='sessionContainer' ref={menuRef}>
      {isMenuOpen && (
        <ul className='user-menu'>
          {isAuthenticated ? (
            <>
              <li onClick={goToAdminDashboard}>Administrar</li>
              <li>Perfil</li>
              <li>Cerrar Sesión</li>
            </>
          ) : (
            <>
              <li onClick={() => Swal.fire('Funcionalidad en Desarrollo', 'Esta funcionalidad estará disponible pronto.', 'info')}>Registrarse</li>
              <li onClick={() => Swal.fire('Funcionalidad en Desarrollo', 'Esta funcionalidad estará disponible pronto.', 'info')}>Iniciar Sesión</li>
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
