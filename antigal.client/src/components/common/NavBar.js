// src/components/common/NavBar.js
import React from 'react';
import { NavLink } from 'react-router-dom';
import PropTypes from 'prop-types';
import CartWidget from './CartWidget';

const NavBar = ({ vertical = false, onLinkClick = () => {} }) => {
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
          <NavLink 
            to="/about" 
            className={({ isActive }) => isActive ? 'active' : undefined}
            onClick={onLinkClick}
          >
            Sobre Nosotros
          </NavLink>
        </li>
        <li>
          <NavLink 
            to="/store" 
            className={({ isActive }) => isActive ? 'active' : undefined}
            onClick={onLinkClick}
          >
            Tienda Física
          </NavLink>
        </li>
        <li>
          <NavLink 
            to="/contact" 
            className={({ isActive }) => isActive ? 'active' : undefined}
            onClick={onLinkClick}
          >
            Contacto
          </NavLink>
        </li>
        <li>
          <NavLink 
            to="/cart" 
            className={({ isActive }) => isActive ? 'active' : undefined}
            onClick={onLinkClick}
          >
            <CartWidget />
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
