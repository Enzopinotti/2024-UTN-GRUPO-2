// src/components/common/Logo.js
import React from 'react';
import { useNavigate } from 'react-router-dom';

const Logo = () => {
  const navigate = useNavigate();

  const handleClick = () => {
    navigate('/'); 
  };

  return (
    <div onClick={handleClick} style={{ cursor: 'pointer' }}>
      <img className='logo' alt='logo de antigal' src='/images/logoAntigal.svg'/>
    </div>
  );
};

export default Logo;
