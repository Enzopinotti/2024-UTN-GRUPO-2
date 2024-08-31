import React from 'react';

const NavBar = () => {
  return (
    <nav>
      <ul>
        <li><a href="/">Inicio</a></li>
        <li><a href="/products">Productos</a></li>
        <li><a href="/cart">Carrito</a></li>
        <li><a href="/login">Login</a></li>
      </ul>
    </nav>
  );
};

export default NavBar;