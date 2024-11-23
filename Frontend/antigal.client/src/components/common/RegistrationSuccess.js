// src/components/common/RegistrationSuccess.js

import React from 'react';
import { Link } from 'react-router-dom';

const RegistrationSuccess = () => {
  return (
    <div className="registration-success-page">
      <div className="registration-success-container">
        <h2>¡Registro Exitoso!</h2>
        <p>
          Hemos enviado un correo electrónico a tu dirección. Por favor, revisa tu correo electrónico y haz clic en el enlace de confirmación para activar tu cuenta.
        </p>
        <Link to="/login">Ir a Iniciar Sesión</Link>
      </div>
    </div>
  );
};

export default RegistrationSuccess;
