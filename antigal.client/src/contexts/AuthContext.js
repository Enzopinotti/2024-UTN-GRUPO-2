import React, { createContext, useState } from 'react';

// Crear el contexto
export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
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
    }
  };

  // Función de logout (opcional, pero útil para manejar la sesión)
  const logout = () => {
    setUser(null);
    localStorage.removeItem('user');
  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
