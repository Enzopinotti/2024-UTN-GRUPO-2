// src/pages/Registro.js
import { useEffect } from 'react';
import { useAuth0 } from '@auth0/auth0-react';

const Registro = () => {
  const { loginWithRedirect } = useAuth0();

  useEffect(() => {
    loginWithRedirect({
      screen_hint: 'signup',
    });
  }, [loginWithRedirect]);

  return (
    <div>
      <p>Redirigiendo al registro...</p>
    </div>
  );
};

export default Registro;
