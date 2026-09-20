// src/components/Callback.js
import { useAuth0 } from '@auth0/auth0-react';

const Callback = () => {
  const { isLoading } = useAuth0();

  return (
    <div>
      {isLoading ? 'Procesando autenticación...' : 'Redirigiendo...'}
    </div>
  );
};

export default Callback;
