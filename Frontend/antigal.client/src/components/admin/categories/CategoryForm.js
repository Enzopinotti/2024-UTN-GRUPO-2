// src/components/admin/categories/CategoryForm.js
import React, { useState, useEffect } from 'react';
import Swal from 'sweetalert2';

const CategoryForm = ({ show, onClose, onSave, category }) => {
  // Estados para los campos del formulario
  const [nombre, setNombre] = useState('');
  const [descripcion, setDescripcion] = useState('');
  const [imagen, setImagen] = useState('');

  useEffect(() => {
    if (category) {
      setNombre(category.nombre);
      setDescripcion(category.descripcion);
      setImagen(category.imagen || '');
    } else {
      setNombre('');
      setDescripcion('');
      setImagen('');
    }
  }, [category]);

  // Manejador para el cambio de imagen
  const handleImageChange = (e) => {
    setImagen(e.target.value);
  };

  // Validaciones del formulario
  const validateForm = () => {
    if (nombre.trim() === '' || descripcion.trim() === '') {
      Swal.fire('Error', 'Todos los campos son obligatorios.', 'error');
      return false;
    }
    return true;
  };

  // Manejador de envío del formulario
  const handleSubmit = (e) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    const newCategory = {
      idCategoria: category ? category.idCategoria : Date.now(),
      nombre,
      descripcion,
      imagen,
    };

    onSave(newCategory);
    onClose();
  };

  // Manejadores para cerrar el modal al hacer clic fuera o en la "X"
  const handleOverlayClick = (event) => {
    if (event.target.className === 'form-overlay') {
      onClose();
    }
  };

  const handleFormClick = (event) => {
    event.stopPropagation();
  };

  // Mover la condición después de los Hooks
  if (!show) {
    return null;
  }

  return (
    <div className="form-overlay" onClick={handleOverlayClick}>
      <div className="form" onClick={handleFormClick}>
        <div className="top-section">
          <h2>{category ? 'Editar Categoría' : 'Crear Nueva Categoría'}</h2>
          <button type="button" className="exit-btn" onClick={onClose}>
            &times;
          </button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className="form-fields">
            <div className="input">
              <label htmlFor="nombre">Nombre de la categoría:</label>
              <input
                type="text"
                id="nombre"
                name="nombre"
                value={nombre}
                onChange={(e) => setNombre(e.target.value)}
                required
              />
            </div>
            <div className="input">
              <label htmlFor="descripcion">Descripción:</label>
              <textarea
                id="descripcion"
                name="descripcion"
                value={descripcion}
                onChange={(e) => setDescripcion(e.target.value)}
                required
              />
            </div>
            <div className="input">
              <label htmlFor="imagen">URL de la imagen:</label>
              <input
                type="text"
                id="imagen"
                name="imagen"
                value={imagen}
                onChange={handleImageChange}
              />
            </div>
          </div>
          <div className="bottom-section">
            <button type="button" onClick={onClose}>
              Cancelar
            </button>
            <button type="submit" className="create-btn">
              {category ? 'Actualizar' : 'Crear'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CategoryForm;
