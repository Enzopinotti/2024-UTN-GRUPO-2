import { useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useForm, useWatch } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import { resetPasswordSchema } from '../../validations/validationSchemas';
import { arePasswordConditionsMet, getPasswordConditions } from '../../utils/passwordConditions';
import DOMPurify from 'dompurify';
import { toast } from 'react-toastify';

const ResetearContrasenia = () => {
  const navigate = useNavigate();
  const { token } = useParams();
  const [showPassword, setShowPassword] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    control,
  } = useForm({
    resolver: yupResolver(resetPasswordSchema),
  });

  const passwordValue = useWatch({
    control,
    name: 'password',
    defaultValue: '',
  });

  const passwordConditions = getPasswordConditions(passwordValue);
  const allConditionsMet = arePasswordConditionsMet(passwordConditions);

  const sanitizeInput = (input) => {
    return DOMPurify.sanitize(input);
  };

  const onSubmit = async (data) => {
    const sanitizedData = {
      token: sanitizeInput(token),
      password: sanitizeInput(data.password),
    };

    try {
      const response = await fetch('https://www.antigal.somee.com/api/auth/resetear', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(sanitizedData),
      });

      if (response.ok) {
        toast.success('Tu contraseña ha sido restablecida con éxito. Puedes iniciar sesión ahora.');
        setTimeout(() => navigate('/login'), 3000);
      } else {
        const errorData = await response.json();
        toast.error(errorData.message || 'Error al restablecer la contraseña.');
      }
    } catch (err) {
      console.error('Error al restablecer la contraseña:', err);
      toast.error('Error al restablecer la contraseña. Inténtalo de nuevo más tarde.');
    }
  };

  // Alternar visibilidad de la contraseña y confirmación de contraseña
  const togglePasswordVisibility = () => setShowPassword((prev) => !prev);

  return (
    <div className="auth-page fondoAuth">
      <div className="auth-container">
        <img src="./icons/iconoAntigal.png" alt="Logo" />
        <h2>Resetear Contraseña</h2>
        <form onSubmit={handleSubmit(onSubmit)}>
          <input
            type="text"
            name="username"
            autoComplete="username"
            style={{ display: 'none' }}
            {...register('username')}
          />

          <div className="form-group">
            <label htmlFor="password">Nueva Contraseña:</label>
            <div className="password-input-wrapper">
              <input
                type={showPassword ? 'text' : 'password'}
                id="password"
                {...register('password')}
                placeholder="Ingresa tu nueva contraseña"
                autoComplete="new-password"
              />
              <div onClick={togglePasswordVisibility} className="toggle-password ojo" aria-label="Mostrar u ocultar contraseña">
                <img
                  src={showPassword ? '/icons/ojo-cerrado.png' : '/icons/ojoAbierto.png'}
                  alt={showPassword ? 'Ocultar contraseña' : 'Mostrar contraseña'}
                />
              </div>
            </div>
            {errors.password && <p className="error-message">{errors.password.message}</p>}
            {!allConditionsMet && (
              <div className={`password-conditions`}>
                <p className={passwordConditions.length ? 'condition met' : 'condition unmet'}>
                  • Al menos 8 caracteres
                </p>
                <p className={passwordConditions.lowercase ? 'condition met' : 'condition unmet'}>
                  • Al menos una letra minúscula
                </p>
                <p className={passwordConditions.uppercase ? 'condition met' : 'condition unmet'}>
                  • Al menos una letra mayúscula
                </p>
                <p className={passwordConditions.number ? 'condition met' : 'condition unmet'}>
                  • Al menos un número
                </p>
                <p className={passwordConditions.specialChar ? 'condition met' : 'condition unmet'}>
                  • Al menos un carácter especial (@$!%*?&)
                </p>
              </div>
            )}
            {allConditionsMet && (
              <p className="password-secure-message">¡La contraseña es segura!</p>
            )}
          </div>

          <div className="form-group">
            <label htmlFor="confirmPassword">Confirmar Contraseña:</label>
            <div className="password-input-wrapper">
              <input
                type="password"
                id="confirmPassword"
                {...register('confirmPassword')}
                placeholder="Confirma tu nueva contraseña"
                autoComplete="new-password"
              />
              
            </div>
            {errors.confirmPassword && (
              <p className="error-message">{errors.confirmPassword.message}</p>
            )}
          </div>

          <button type="submit" className="cta-button primary">
            Restablecer Contraseña
          </button>
        </form>
      </div>
    </div>
  );
};

export default ResetearContrasenia;
