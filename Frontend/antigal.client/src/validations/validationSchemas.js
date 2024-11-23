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
// **Nuevo: Esquema de Validación para Productos**
export const productSchema = Yup.object().shape({
  nombre: Yup.string()
    .required('El nombre del producto es obligatorio')
    .min(2, 'Debe tener al menos 2 caracteres')
    .max(100, 'No puede exceder 100 caracteres'),
  precio: Yup.number()
    .typeError('El precio debe ser un número')
    .required('El precio es obligatorio')
    .positive('El precio debe ser un número positivo'),
  categoria: Yup.string()
    .required('La categoría es obligatoria'),
  descripcion: Yup.string()
    .required('La descripción es obligatoria')
    .min(10, 'Debe tener al menos 10 caracteres'),
  codigoBarras: Yup.string()
    .matches(/^\d+$/, 'El código de barras debe contener solo números')
    .optional(),
  marca: Yup.string()
    .optional()
    .max(50, 'No puede exceder 50 caracteres'),
  stock: Yup.number()
    .typeError('El stock debe ser un número')
    .required('El stock es obligatorio')
    .integer('El stock debe ser un número entero')
    .min(0, 'El stock no puede ser negativo'),
  disponible: Yup.number()
    .oneOf([0, 1], 'Disponible debe ser 0 (No) o 1 (Sí)')
    .required('El estado de disponibilidad es obligatorio'),
  destacado: Yup.number()
    .oneOf([0, 1], 'Destacado debe ser 0 (No) o 1 (Sí)')
    .required('El estado de destacado es obligatorio'),
  imagenes: Yup.mixed()
    .test('maxImages', 'No puedes subir más de 6 imágenes', (value) => {
      if (value && value.length > 6) {
        return false;
      }
      return true;
    }),
});