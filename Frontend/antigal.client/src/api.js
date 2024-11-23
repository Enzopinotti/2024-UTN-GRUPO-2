// src/api.js
import { useContext, useCallback } from 'react';
import { AuthContext } from './contexts/AuthContext';
import Swal from 'sweetalert2';

export const useApi = () => {
  const { auth, logout } = useContext(AuthContext);
  const token = auth.accessToken;

  const fetchWithAuth = useCallback(async (url, options = {}) => {
    const headers = {
      ...options.headers,
      'Authorization': `Bearer ${token}`,
    };

    // Log de los headers para verificar
    console.log('Enviando solicitud a:', url);
    console.log('Headers de solicitud:', headers);

    try {
      const response = await fetch(url, { ...options, headers });

      if (response.status === 401) {
        // Token inválido o expirado
        Swal.fire('Error', 'Sesión expirada. Por favor, inicia sesión nuevamente.', 'error');
        logout();
        return null;
      }

      return response;
    } catch (error) {
      console.error('Error en fetchWithAuth:', error);
      throw error;
    }
  }, [token, logout]);

  return { fetchWithAuth };
};
