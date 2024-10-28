// src/pages/Registro.jsx
import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { toast } from 'react-toastify';

const Registro = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    userName: '',
    email: '',
    password: '',
    fullName: '',
  });
  const [error, setError] = useState('');
  const [mensaje, setMensaje] = useState('');

  const { userName, email, password, fullName } = formData;

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleRegistro = async (e) => {
    e.preventDefault();
    setError('');
    setMensaje('');

    try {
      const response = await fetch('https://localhost:7255/api/Auth/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          userName,
          email,
          password,
          fullName,
        }),
      });

      if (response.ok) {
        setMensaje('¡Usuario registrado con éxito! Ahora puedes iniciar sesión.');
        setFormData({
          userName: '',
          email: '',
          password: '',
          fullName: '',
        });
        toast.success('¡Usuario registrado con éxito!');
        setTimeout(() => navigate('/login'), 3000);
      } else {
        const errorData = await response.json();
        setError(errorData.message || 'Error al registrarse.');
        toast.error(errorData.message || 'Error al registrarse.');
      }
    } catch (err) {
      console.error('Error al registrarse:', err);
      setError('Error al registrarse. Inténtalo de nuevo más tarde.');
      toast.error('Error al registrarse. Inténtalo de nuevo más tarde.');
    }
  };

  return (
    <div className="auth-page fondoAuth">
      <div className="auth-container">
        <img  src='./icons/iconoAntigal.png' alt="Logo" />
        <h2>Registro</h2>
        <form onSubmit={handleRegistro}>
          <div className="form-group">
            <label htmlFor="userName">Nombre de Usuario:</label>
            <input
              type="text"
              id="userName"
              name="userName"
              value={userName}
              onChange={handleChange}
              required
              placeholder="Ingresa tu nombre de usuario"
            />
          </div>
          <div className="form-group">
            <label htmlFor="email">Correo Electrónico:</label>
            <input
              type="email"
              id="email"
              name="email"
              value={email}
              onChange={handleChange}
              required
              placeholder="Ingresa tu correo electrónico"
            />
          </div>
          <div className="form-group">
            <label htmlFor="password">Contraseña:</label>
            <input
              type="password"
              id="password"
              name="password"
              value={password}
              onChange={handleChange}
              required
              placeholder="Ingresa tu contraseña"
            />
          </div>
          <div className="form-group">
            <label htmlFor="fullName">Nombre Completo:</label>
            <input
              type="text"
              id="fullName"
              name="fullName"
              value={fullName}
              onChange={handleChange}
              required
              placeholder="Ingresa tu nombre completo"
            />
          </div>
          <button type="submit" className="cta-button primary">Registrarse</button>
        </form>
        <div className="auth-links">
          <Link to="/login">¿Ya tienes una cuenta? Inicia Sesión</Link>
        </div>
        {mensaje && <p className="mensaje-exito">{mensaje}</p>}
        {error && <p className="mensaje-error">{error}</p>}
      </div>
    </div>
  );
};

export default Registro;
