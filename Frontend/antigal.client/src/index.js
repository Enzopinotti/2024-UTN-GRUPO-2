import React from 'react';
import ReactDOM from 'react-dom/client';
import './styles/css/index.css';
import App from './App';
import { Auth0Provider } from '@auth0/auth0-react';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
    <Auth0Provider
      domain="dev-y32n3zbsc0hwziwi.us.auth0.com"
      clientId="Sw8C9MwxDLv2cCN3BjrkpF7LK5cHVg5d"
      redirectUri={`${window.location.origin}/callback`}
      audience="TU_API_AUDIENCE" // Si estás utilizando API en el backend
      scope="openid profile email"
    >

      <React.StrictMode>
        <App />
      </React.StrictMode>
    </Auth0Provider>
);

