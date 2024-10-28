// src/pages/Login.jsx
import React, { useState, useContext } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { AuthContext } from '../../contexts/AuthContext';
import { toast } from 'react-toastify';

const Login = () => {
  const navigate = useNavigate();
  const { login } = useContext(AuthContext);
  const [formData, setFormData] = useState({
    userName: '',
    password: '',
  });
  const [error, setError] = useState('');

  const { userName, password } = formData;

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleLogin = async (e) => {
    e.preventDefault();
    setError('');

    try {
      const response = await fetch('https://localhost:7255/api/Auth/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          userName,
          password,
        }),
      });

      if (response.ok) {
        const data = await response.json();
        const { accessToken, refreshToken } = data;
        login(accessToken, refreshToken); // Si implementas Refresh Tokens
        toast.success('¡Inicio de sesión exitoso!');
        //navigate('/');    
        console.log(data)
      } else {
        const errorData = await response.json();
        setError(errorData.message || 'Credenciales inválidas');
        toast.error(errorData.message || 'Credenciales inválidas');
      }
    } catch (err) {
      console.error('Error al iniciar sesión:', err);
      setError('Error al iniciar sesión. Inténtalo de nuevo más tarde.');
      toast.error('Error al iniciar sesión. Inténtalo de nuevo más tarde.');
    }
  };

  return (
    <div className="auth-page fondoAuth">
      <div className="auth-container">
        <img  src='./icons/iconoAntigal.png' alt="Logo" />

        <h2>Inicio de Sesión</h2>
        <form onSubmit={handleLogin}>
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
          <button type="submit" className="cta-button primary">Iniciar Sesión</button>
        </form>
        <div className="auth-links">
          <Link to="/recuperar-contrasena">¿Olvidaste tu contraseña?</Link>
        </div>
        {error && <p className="mensaje-error">{error}</p>}
      </div>
    </div>
  );
};

export default Login;