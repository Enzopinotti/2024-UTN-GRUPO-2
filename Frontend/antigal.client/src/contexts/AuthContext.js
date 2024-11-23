<<<<<<< HEAD
import React, { createContext, useState } from 'react';
=======
// src/contexts/AuthContext.js
import React, { createContext, useState, useEffect, useCallback } from 'react';
import { jwtDecode } from 'jwt-decode'; // Importación corregida
>>>>>>> FrontEnd

// Crear el contexto
export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
<<<<<<< HEAD
  const [user, setUser] = useState(null);

  // Función de login
  const login = async (userData) => {
    // Aquí iría la lógica para autenticar al usuario, como una llamada a una API.
    // Simularemos la autenticación guardando los datos del usuario en el estado.
    
    try {
      // Simulando una autenticación exitosa
      setUser(userData);

      // Puedes almacenar los datos del usuario en localStorage si quieres mantener la sesión
      localStorage.setItem('user', JSON.stringify(userData));
    } catch (error) {
      // Manejar errores de autenticación (ej., credenciales incorrectas)
      throw new Error('Error de autenticación');
=======
  const [auth, setAuth] = useState({
    accessToken: null,
    //refreshToken: null, // Opcional: Si implementas Refresh Tokens
    user: null,
  });

  // Definir la función 'logout' primero
  const logout = useCallback(() => {
    setAuth({
      accessToken: null,
      //refreshToken: null,
      user: null,
    });
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    console.log('Usuario ha cerrado sesión.');
  }, []);

  // Definir la función 'login' a continuación
  const login = useCallback((accessToken, refreshToken = null) => {
    const decoded = jwtDecode(accessToken);
    setAuth({
      accessToken,
      refreshToken,
      user: decoded,
    });
    localStorage.setItem('accessToken', accessToken);
    if (refreshToken) {
      localStorage.setItem('refreshToken', refreshToken);
>>>>>>> FrontEnd
    }
    console.log('Usuario ha iniciado sesión:', decoded);
  }, []);

<<<<<<< HEAD
  // Función de logout (opcional, pero útil para manejar la sesión)
  const logout = () => {
    setUser(null);
    localStorage.removeItem('user');
  };
=======
  // Ahora, utilizar 'logout' en el useEffect
  useEffect(() => {
    const accessToken = localStorage.getItem('accessToken');
    //const refreshToken = localStorage.getItem('refreshToken'); // Opcional
    if (accessToken) {
      try {
        const decoded = jwtDecode(accessToken);
        // Verificar si el token no ha expirado
        if (decoded.exp * 1000 > Date.now()) {
          setAuth({
            accessToken,
            //refreshToken,
            user: decoded,
          });
          console.log('Token válido encontrado:', decoded);
        } else {
          // Token expirado
          console.log('Token expirado.');
          logout();
        }
      } catch (error) {
        console.error('Error al decodificar el token:', error);
        logout();
      }
    }
  }, [logout]); // 'logout' ya está definido y memoizado
>>>>>>> FrontEnd

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
