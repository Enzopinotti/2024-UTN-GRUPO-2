import React from 'react';
import { NavLink } from 'react-router-dom';
import PropTypes from 'prop-types';
import Swal from 'sweetalert2'; // Importamos SweetAlert2

const NavBar = ({ vertical = false, onLinkClick = () => {} }) => {

  // Función para mostrar mensaje de "Funcionalidad en Desarrollo"
  const showDevelopmentAlert = (event) => {
    event.preventDefault(); // Evitar la navegación por ahora
    Swal.fire({
      title: 'Funcionalidad en Desarrollo',
      text: 'Esta sección estará disponible pronto.',
      icon: 'info',
      confirmButtonText: 'Cerrar'
    });
  };

  return (
    <nav className={vertical ? 'nav-vertical' : 'nav-horizontal'}>
      <ul>
        <li>
          <NavLink 
            to="/" 
            className={({ isActive }) => isActive ? 'active' : undefined}
            onClick={onLinkClick}
          >
            Inicio
          </NavLink>
        </li>
        <li>
          <NavLink 
            to="/products" 
            className={({ isActive }) => isActive ? 'active' : undefined}
            onClick={onLinkClick}
          >
            Productos
          </NavLink>
        </li>
        <li>
          {/* Mantener NavLink pero con alerta de desarrollo */}
          <NavLink 
            to="/about" 
            className={({ isActive }) => isActive ? 'active' : undefined}
            onClick={showDevelopmentAlert}  // Mostrar mensaje de desarrollo
          >
            Sobre Nosotros
          </NavLink>
        </li>
        <li>
          <NavLink 
            to="/store" 
            className={({ isActive }) => isActive ? 'active' : undefined}
            onClick={showDevelopmentAlert}  // Mostrar mensaje de desarrollo
          >
            Tienda Física
          </NavLink>
        </li>
        <li>
          <NavLink 
            to="/contact" 
            className={({ isActive }) => isActive ? 'active' : undefined}
            onClick={showDevelopmentAlert}  // Mostrar mensaje de desarrollo
          >
            Contacto
          </NavLink>
        </li>
      </ul>
    </nav>  
  );
};

// Definir los tipos de prop
NavBar.propTypes = {
  vertical: PropTypes.bool,
  onLinkClick: PropTypes.func,
};

export default NavBar;
