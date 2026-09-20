// src/contexts/AuthContext.js
import { createContext, useCallback, useState } from "react";
import { jwtDecode } from "jwt-decode";

export const AuthContext = createContext();

const createUnauthenticatedState = () => ({
  accessToken: null,
  user: null,
});

const clearPersistedCredentials = () => {
  localStorage.removeItem("accessToken");
  localStorage.removeItem("refreshToken");
};

const readInitialAuth = () => {
  const accessToken = localStorage.getItem("accessToken");

  if (!accessToken) {
    return createUnauthenticatedState();
  }

  try {
    const decoded = jwtDecode(accessToken);

    if (decoded.exp * 1000 > Date.now()) {
      console.log("Token válido encontrado:", decoded);
      return {
        accessToken,
        user: decoded,
      };
    }

    console.log("Token expirado.");
  } catch (error) {
    console.error("Error al decodificar el token:", error);
  }

  clearPersistedCredentials();
  return createUnauthenticatedState();
};

export const AuthProvider = ({ children }) => {
  const [auth, setAuth] = useState(readInitialAuth);

  const logout = useCallback(() => {
    setAuth(createUnauthenticatedState());
    clearPersistedCredentials();
    console.log("Usuario ha cerrado sesión.");
  }, []);

  const login = useCallback((accessToken, refreshToken = null) => {
    const decoded = jwtDecode(accessToken);
    setAuth({
      accessToken,
      refreshToken,
      user: decoded,
    });
    localStorage.setItem("accessToken", accessToken);
    if (refreshToken) {
      localStorage.setItem("refreshToken", refreshToken);
    }
    console.log("Usuario ha iniciado sesión:", decoded);
  }, []);

  return (
    <AuthContext.Provider value={{ auth, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};
