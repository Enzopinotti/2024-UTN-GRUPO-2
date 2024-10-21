import React, { useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../../contexts/AuthContext';

const Login = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const { login } = useContext(AuthContext);  // Obtener la función de login del AuthContext
  const navigate = useNavigate();  // Hook de react-router-dom para redirigir después del login

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    try {
      // Lógica para autenticar al usuario (esto puede ser una llamada a una API)
      const userData = { username, password };
      await login(userData);  // Supone que login es una función asíncrona que realiza la autenticación

      // Si la autenticación es exitosa, redirigir a la página de inicio
      navigate('/');
    } catch (err) {
      // Manejo de errores (mostrar mensaje de error al usuario)
      setError('Credenciales incorrectas. Por favor, inténtelo de nuevo.');
    }
  };

  return (
    <div>
      <h2>Iniciar Sesión</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="username">Usuario:</label>
          <input
            type="text"
            id="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
        </div>
        <div>
          <label htmlFor="password">Contraseña:</label>
          <input
            type="password"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <button type="submit">Iniciar Sesión</button>
      </form>
    </div>
  );
};

export default Login;
