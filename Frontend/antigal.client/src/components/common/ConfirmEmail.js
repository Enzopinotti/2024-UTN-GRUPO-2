// src/components/common/ConfirmEmail.js

import React, { useEffect, useState } from 'react';
import { useSearchParams, Link, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';

const ConfirmEmail = () => {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [status, setStatus] = useState('loading'); // 'loading', 'success', 'error'
  const [message, setMessage] = useState('');

  useEffect(() => {
    const confirmEmail = async () => {
      const userId = searchParams.get('userId');
      const token = searchParams.get('token');

      if (!userId || !token) {
        setStatus('error');
        setMessage('Datos de confirmación inválidos.');
        toast.error('Datos de confirmación inválidos.');
        return;
      }

      try {
        const response = await fetch(`https://www.antigal.somee.com/api/accounts/confirm-email?userId=${userId}&token=${token}`, {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
          },
        });

        if (response.ok) {
          const data = await response.json();
          setStatus('success');
          setMessage(data.Message || 'Email confirmado exitosamente.');
          toast.success(data.Message || 'Email confirmado exitosamente.');
          // Opcional: Redirigir al login después de unos segundos
          setTimeout(() => navigate('/login'), 5000);
        } else {
          const errorData = await response.json();
          setStatus('error');
          setMessage(errorData.Message || 'Error al confirmar el email.');
          toast.error(errorData.Message || 'Error al confirmar el email.');
        }
      } catch (error) {
        console.error('Error al confirmar el email:', error);
        setStatus('error');
        setMessage('Error al confirmar el email. Inténtalo de nuevo más tarde.');
        toast.error('Error al confirmar el email. Inténtalo de nuevo más tarde.');
      }
    };

    confirmEmail();
  }, [searchParams, navigate]);

  return (
    <div className="confirm-email-page">
      <div className="confirm-email-container">
        {status === 'loading' && <p>Confirmando tu email, por favor espera...</p>}
        {status === 'success' && (
          <div>
            <h2>¡Email Confirmado!</h2>
            <p>{message}</p>
            <Link to="/login" className="cta-button primary">Iniciar Sesión</Link>
          </div>
        )}
        {status === 'error' && (
          <div>
            <h2>Error al Confirmar Email</h2>
            <p>{message}</p>
            <Link to="/register" className="cta-button secondary">Regístrate nuevamente</Link>
          </div>
        )}
      </div>
    </div>
  );
};

export default ConfirmEmail;
