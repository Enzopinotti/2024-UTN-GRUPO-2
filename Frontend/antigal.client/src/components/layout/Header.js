import React, { useState, useEffect } from 'react';
import NavBar from '../common/NavBar';
import Logo from '../common/Logo';
import CartWidget from '../common/CartWidget';
import LupaWidget from '../common/LupaWidget';
import MenuHamburger from '../common/MenuHamburger';
import CloseIcon from '../common/CloseIcon';
import SocialMedia from '../common/SocialMedia';
import UserIcon from '../common/UserIconDos';
import Swal from 'sweetalert2'; // Importamos SweetAlert2
import SearchBar from '../common/SearchBar';
import SearchBarMobile from '../common/SearchBarMobile';
const Header = () => {
  const [isMobile, setIsMobile] = useState(window.innerWidth <= 768);
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const [isMenuVisible, setIsMenuVisible] = useState(false);
  const [isSearchBarVisible, setIsSearchBarVisible] = useState(false);
  // Función para mostrar mensaje de "Funcionalidad en Desarrollo"
  const showDevelopmentAlert = () => {
    Swal.fire({
      title: 'Funcionalidad en Desarrollo',
      text: 'Esta funcionalidad estará disponible pronto.',
      icon: 'info',
      confirmButtonText: 'Cerrar'
    });
  };
  const toggleSearchBar=()=>{
    setIsSearchBarVisible(!isSearchBarVisible)
  }

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
          {/* Alerta de "Funcionalidad en Desarrollo" al hacer clic en el carrito */}
          <CartWidget onClick={showDevelopmentAlert} />
          
          <Logo toggleMenu={toggleMenu} isMenuOpen={isMenuOpen} />
          <MenuHamburger onClick={toggleMenu} />
          {isMenuVisible && (
            <div className={`mobile-menu ${isMenuOpen ? 'open' : 'close'}`}>
              <article className='cierreYlogo'>
                <Logo toggleMenu={toggleMenu} isMenuOpen={isMenuOpen} />
                <CloseIcon onClick={toggleMenu} />
              </article>
              <UserIcon />                  
              <NavBar vertical={true} onLinkClick={toggleMenu} /> 
             <SearchBarMobile/>
              <SocialMedia />
            </div>
          )}
        </div>
      ) : (
        <div className='header-desktop'>
          <Logo />
          <NavBar />
          <div className='widgets'>
            {/* Alerta de "Funcionalidad en Desarrollo" al hacer clic en la lupa */}
            <LupaWidget onClick={toggleSearchBar} />
            <CartWidget onClick={showDevelopmentAlert} />
            <UserIcon />
          </div>
        </div>
      )}
     {isSearchBarVisible && <SearchBar isVisible={isSearchBarVisible} onClose={toggleSearchBar } />}      </header>
  );
};

export default Header;
