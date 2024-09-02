// src/components/common/MenuHamburger.js
import React from 'react';

const MenuHamburger = ({ onClick }) => {
  return (
    <div onClick={onClick}> 
      <img className='hamburgesa' src='/images/hamburgesa.png' alt='menu de hamburguesa' />
    </div>
  );
}

export default MenuHamburger;