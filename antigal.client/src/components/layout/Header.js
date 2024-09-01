// src/components/layout/Header.js
import React, { useState, useEffect } from 'react';
import NavBar from '../common/NavBar';
import Logo from '../common/Logo';
import CartWidget from '../common/CartWidget';
import LupaWidget from '../common/LupaWidget';
import MenuHamburger from '../common/MenuHamburger';

const Header = () => {
  const [isMobile, setIsMobile] = useState(window.innerWidth <= 768);
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
  };

  useEffect(() => {
    const handleResize = () => {
      setIsMobile(window.innerWidth <= 768);
    };

    window.addEventListener('resize', handleResize);

    // Cleanup function
    return () => {
      window.removeEventListener('resize', handleResize);
    };
  }, []);

  return (
    <header className='header'>
      {isMobile ? (
        <div className='header-mobile'>
          <Logo />
          <MenuHamburger onClick={toggleMenu} />
          {isMenuOpen && (
            <div className='mobile-menu'>
              <NavBar vertical={true} />
              <button className='login-button'>Iniciar Sesión</button>
              <button className='register-button'>Registrarse</button>
            </div>
          )}
        </div>
      ) : (
        <div className='header-desktop'>
          <Logo />
          <NavBar />
          <div className='widgets'>
            <CartWidget />
            <LupaWidget />
          </div>
        </div>
      )}
    </header>
  );
};

export default Header;
