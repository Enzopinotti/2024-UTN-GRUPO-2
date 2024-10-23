// src/pages/Profile.js
import React from 'react';
import { useAuth0 } from '@auth0/auth0-react';

const Profile = () => {
  const { user, isAuthenticated, isLoading } = useAuth0();

  if (isLoading) {
    return <div>Cargando...</div>;
  }

  if (!isAuthenticated) {
    return <div>No estás autenticado</div>;
  }

  return (
    <div className="profile-page">
      <h2>Perfil de Usuario</h2>
      <img src={user.picture} alt="Profile" />
      <p>Nombre: {user.name}</p>
      <p>Email: {user.email}</p>
    </div>
  );
};

export default Profile;
