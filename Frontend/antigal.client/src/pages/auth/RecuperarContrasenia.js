// src/pages/RecuperarContraseña.jsx
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';


const RecuperarContrasenia = () => {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [mensaje, setMensaje] = useState('');
  const [error, setError] = useState('');

  const handleRecuperar = async (e) => {
    e.preventDefault();
    setError('');
    setMensaje('');

    try {
      const response = await fetch('https://tu-backend.com/api/auth/recuperar', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email }),
      });

      if (response.ok) {
        setMensaje('Se ha enviado un enlace para restablecer tu contraseña a tu correo electrónico.');
      } else {
        const errorData = await response.json();
        setError(errorData.message || 'Error al solicitar recuperación de contraseña.');
      }
    } catch (err) {
      console.error('Error al solicitar recuperación de contraseña:', err);
      setError('Error al solicitar recuperación de contraseña. Inténtalo de nuevo más tarde.');
    }
  };

  return (
    <div className="auth-page fondoAuth">
      <div className="auth-container">
        <img  src='./icons/iconoAntigal.png' alt="Logo" />
        <h2>Recuperar Contraseña</h2>
        <form onSubmit={handleRecuperar}>
          <div className="form-group">
            <label htmlFor="email">Correo Electrónico:</label>
            <input
              type="email"
              id="email"
              name="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              placeholder="Ingresa tu correo electrónico"
            />
          </div>
          <button type="submit" className="cta-button primary">Enviar Enlace</button>
        </form>
        {mensaje && <p className="mensaje-exito">{mensaje}</p>}
        {error && <p className="mensaje-error">{error}</p>}
      </div>
    </div>
  );
};

export default RecuperarContrasenia;
