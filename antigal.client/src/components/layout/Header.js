// src/components/layout/Header.js
import React, { useState, useEffect } from 'react';
import NavBar from '../common/NavBar';
import Logo from '../common/Logo';
import CartWidget from '../common/CartWidget';
import LupaWidget from '../common/LupaWidget';
import MenuHamburger from '../common/MenuHamburger';
import CloseIcon from '../common/CloseIcon';
import SocialMedia from '../common/SocialMedia';
import UserIcon from '../common/UserIconDos';

const Header = () => {
  const [isMobile, setIsMobile] = useState(window.innerWidth <= 768);
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [isMenuVisible, setIsMenuVisible] = useState(false);

  const toggleMenu = () => {
    if (!isMenuOpen) {
      setIsMenuVisible(true);   
      setIsMenuOpen(true);    
    } else {
      setIsMenuOpen(false);   
    }
  };

  useEffect(() => {
    const handleResize = () => {
      const mobile = window.innerWidth <= 800;
      setIsMobile(mobile);
      if (!mobile) {
        setIsMenuOpen(false);
        setIsMenuVisible(false);
      }
    };

    window.addEventListener('resize', handleResize);

    return () => {
      window.removeEventListener('resize', handleResize);
    };
  }, []);

  useEffect(() => {
    if (!isMenuOpen && isMenuVisible) {
      const timer = setTimeout(() => {
        setIsMenuVisible(false);
      }, 300);

      return () => clearTimeout(timer);
    }
  }, [isMenuOpen, isMenuVisible]);

  return (
    <header className='header'>
      {isMobile ? (
        <div className='header-mobile'>
          <Logo />
          <MenuHamburger onClick={toggleMenu} />
          {isMenuVisible && (
            <div className={`mobile-menu ${isMenuOpen ? 'open' : 'close'}`}>
              <article className='cierreYlogo'>
                <Logo />
                <CloseIcon onClick={toggleMenu} />
              </article>
              <UserIcon />                  
              <NavBar vertical={true} onLinkClick={toggleMenu} /> 
              <SocialMedia />
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
