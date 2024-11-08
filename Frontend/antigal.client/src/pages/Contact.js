// src/pages/Contact.jsx
import React, { useState } from 'react';
import { toast } from 'react-toastify';

const Contact = () => {
  const [formData, setFormData] = useState({
    nombre: '',
    email: '',
    asunto: '',
    mensaje: '',
  });

  const [errors, setErrors] = useState({});

  // Manejar cambios en los campos del formulario
  const handleChange = (e) => {
    const { name, value } = e.target;

    setFormData((prevData) => ({
      ...prevData,
      [name]: value,
    }));

    // Limpiar errores al modificar los campos
    setErrors((prevErrors) => ({
      ...prevErrors,
      [name]: '',
    }));
  };

  // Validar el formulario
  const validate = () => {
    const newErrors = {};

    if (!formData.nombre.trim()) newErrors.nombre = 'El nombre es obligatorio.';
    if (!formData.email.trim()) {
      newErrors.email = 'El correo electrónico es obligatorio.';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'El correo electrónico es inválido.';
    }
    if (!formData.asunto.trim()) newErrors.asunto = 'El asunto es obligatorio.';
    if (!formData.mensaje.trim()) newErrors.mensaje = 'El mensaje es obligatorio.';

    return newErrors;
  };

  // Manejar el envío del formulario
  const handleSubmit = async (e) => {
    e.preventDefault();

    const validationErrors = validate();
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    try {
      // Enviar los datos a json-server
      const response = await fetch(`http://localhost:5000/contacto`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ ...formData, fecha: new Date().toISOString() }),
      });

      if (response.ok) {
        // Limpiar el formulario
        setFormData({
          nombre: '',
          email: '',
          asunto: '',
          mensaje: '',
        });
        toast.success('¡Tu mensaje ha sido enviado exitosamente!');
      } else {
        const errorData = await response.json();
        toast.error(errorData.message || 'Hubo un error al enviar tu mensaje.');
      }
    } catch (error) {
      console.error('Error al enviar el mensaje:', error);
      toast.error('Hubo un error al enviar tu mensaje. Inténtalo de nuevo más tarde.');
    }
  };

  return (
    <div className="contacto">
      <div className="contacto-container">
        <h1>Contacto</h1>
        <form onSubmit={handleSubmit} noValidate>
          <div className="form-group">
            <label htmlFor="nombre">Nombre:</label>
            <input
              type="text"
              id="nombre"
              name="nombre"
              value={formData.nombre}
              onChange={handleChange}
              placeholder="Ingresa tu nombre"
              className={errors.nombre ? 'error' : ''}
            />
            {errors.nombre && <span className="error-message">{errors.nombre}</span>}
          </div>

          <div className="form-group">
            <label htmlFor="email">Correo Electrónico:</label>
            <input
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              placeholder="Ingresa tu correo electrónico"
              className={errors.email ? 'error' : ''}
            />
            {errors.email && <span className="error-message">{errors.email}</span>}
          </div>

          <div className="form-group">
            <label htmlFor="asunto">Asunto:</label>
            <input
              type="text"
              id="asunto"
              name="asunto"
              value={formData.asunto}
              onChange={handleChange}
              placeholder="Asunto de tu mensaje"
              className={errors.asunto ? 'error' : ''}
            />
            {errors.asunto && <span className="error-message">{errors.asunto}</span>}
          </div>

          <div className="form-group">
            <label htmlFor="mensaje">Mensaje:</label>
            <textarea
              id="mensaje"
              name="mensaje"
              value={formData.mensaje}
              onChange={handleChange}
              placeholder="Escribe tu mensaje aquí"
              className={errors.mensaje ? 'error' : ''}
              rows="5"
            ></textarea>
            {errors.mensaje && <span className="error-message">{errors.mensaje}</span>}
          </div>

          <button type="submit" className="cta-button primary">
            Enviar Mensaje
          </button>
        </form>
      </div>
    </div>
  );
};

export default Contact;