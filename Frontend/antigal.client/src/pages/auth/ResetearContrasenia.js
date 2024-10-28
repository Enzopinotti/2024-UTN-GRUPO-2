// src/pages/ResetearContraseña.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';

const ResetearContrasenia = () => {
  const navigate = useNavigate();
  const { token } = useParams(); // Suponiendo que el token viene en la URL
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [mensaje, setMensaje] = useState('');
  const [error, setError] = useState('');

  const handleResetear = async (e) => {
    e.preventDefault();
    setError('');
    setMensaje('');

    if (password !== confirmPassword) {
      setError('Las contraseñas no coinciden.');
      return;
    }

    try {
      const response = await fetch('https://tu-backend.com/api/auth/resetear', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ token, password }),
      });

      if (response.ok) {
        setMensaje('Tu contraseña ha sido restablecida con éxito. Puedes iniciar sesión ahora.');
        setTimeout(() => navigate('/login'), 3000);
      } else {
        const errorData = await response.json();
        setError(errorData.message || 'Error al restablecer la contraseña.');
      }
    } catch (err) {
      console.error('Error al restablecer la contraseña:', err);
      setError('Error al restablecer la contraseña. Inténtalo de nuevo más tarde.');
    }
  };

  return (
    <div className="auth-page fondoAuth">
      <div className="auth-container">
        <img  src='./icons/iconoAntigal.png' alt="Logo" />
        <h2>Resetear Contraseña</h2>
        <form onSubmit={handleResetear}>
           
            <div className="form-group">
                <label htmlFor="password">Nueva Contraseña:</label>
                <input
                type="password"
                id="password"
                name="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
                placeholder="Ingresa tu nueva contraseña"
                />
            </div>
            <div className="form-group">
                <label htmlFor="confirmPassword">Confirmar Contraseña:</label>
                <input
                type="password"
                id="confirmPassword"
                name="confirmPassword"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                required
                placeholder="Confirma tu nueva contraseña"
                />
            </div>
          <button type="submit" className="cta-button primary">Restablecer Contraseña</button>
        </form>
        {mensaje && <p className="mensaje-exito">{mensaje}</p>}
        {error && <p className="mensaje-error">{error}</p>}
      </div>
    </div>
  );
};

export default ResetearContrasenia;
