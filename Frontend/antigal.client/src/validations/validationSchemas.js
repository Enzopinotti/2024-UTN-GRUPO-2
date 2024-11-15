// src/validations/validationSchemas.js
import * as Yup from 'yup';

export const registrationSchema = Yup.object().shape({
  firstName: Yup.string()
    .required('El nombre de usuario es obligatorio')
    .min(4, 'Debe tener al menos 4 caracteres')
    .max(20, 'No puede exceder 20 caracteres')
    .matches(/^[a-zA-Z0-9]+$/, 'Solo puede contener letras y números'),
  lastName: Yup.string() // Cambiado de fullName a lastName
    .required('El apellido es obligatorio')
    .min(2, 'Debe tener al menos 2 caracteres')
    .max(50, 'No puede exceder 50 caracteres'),
  email: Yup.string()
    .required('El correo electrónico es obligatorio')
    .email('Debe ser un correo electrónico válido'),
  password: Yup.string()
    .required('La contraseña es obligatoria')
    .min(8, 'Debe tener al menos 8 caracteres')
    .matches(/[a-z]/, 'Debe contener al menos una letra minúscula')
    .matches(/[A-Z]/, 'Debe contener al menos una letra mayúscula')
    .matches(/\d/, 'Debe contener al menos un número')
    .matches(/[@$!%*?&]/, 'Debe contener al menos un carácter especial (@$!%*?&)'),
  confirmPassword: Yup.string()
    .required('La confirmación de contraseña es obligatoria')
    .oneOf([Yup.ref('password'), null], 'Las contraseñas no coinciden'),
});


export const loginSchema = Yup.object().shape({
    email: Yup.string()
      .required('El correo electrónico es obligatorio')
      .email('Debe ser un correo electrónico válido'),
    password: Yup.string()
      .required('La contraseña es obligatoria')
      .min(8, 'Debe tener al menos 8 caracteres'),
});

export const recoverPasswordSchema = Yup.object().shape({
    email: Yup.string()
      .required('El correo electrónico es obligatorio')
      .email('Debe ser un correo electrónico válido'),
});

export const contactSchema = Yup.object().shape({
    nombre: Yup.string()
      .required('El nombre es obligatorio')
      .min(2, 'Debe tener al menos 2 caracteres'),
    email: Yup.string()
      .required('El correo electrónico es obligatorio')
      .email('Debe ser un correo electrónico válido'),
    asunto: Yup.string()
      .required('El asunto es obligatorio')
      .min(5, 'Debe tener al menos 5 caracteres'),
    mensaje: Yup.string()
      .required('El mensaje es obligatorio')
      .min(10, 'Debe tener al menos 10 caracteres'),
});

export const resetPasswordSchema = Yup.object().shape({
    password: Yup.string()
      .required('La contraseña es obligatoria')
      .min(8, 'Debe tener al menos 8 caracteres')
      .matches(/[a-z]/, 'Debe contener al menos una letra minúscula')
      .matches(/[A-Z]/, 'Debe contener al menos una letra mayúscula')
      .matches(/\d/, 'Debe contener al menos un número')
      .matches(/[@$!%*?&]/, 'Debe contener al menos un carácter especial (@$!%*?&)'),
    confirmPassword: Yup.string()
      .required('La confirmación de contraseña es obligatoria')
      .oneOf([Yup.ref('password'), null], 'Las contraseñas no coinciden'),
});