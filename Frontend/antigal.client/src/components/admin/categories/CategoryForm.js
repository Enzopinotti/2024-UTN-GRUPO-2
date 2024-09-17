import React from "react";

const CategoryForm = ({ show, onClose }) => {
  if (!show) {
    return null;
  }
  
  const handleOverlayClick = (event) => {
    if (event.target.className === "form-overlay") {
      onClose();
    }
  };

  
  const handleFormClick = (event) => {
    event.stopPropagation();
  };

  return (
    <div className="form-overlay" onClick={handleOverlayClick}>
      <div className="form" onClick={handleFormClick}>
        <div className="top-section">
          <h2>Crear nueva categoría </h2>
          <button type="button" className="exit-btn" onClick={onClose}>
            <i className="fa-solid fa-xmark"></i>
          </button>
        </div>
        <form>
          <div className="input">
            <label htmlFor="nombre">Nombre de la categoría:</label>
            <input type="text" id="nombre" name="nombre" required />
          </div>
          <div className="input">
            <label htmlFor="descripcion">Descripción:</label>
            <textarea id="descripcion" name="descripcion" required />
          </div>
          <div className="input">
            <label htmlFor="imagen">Subir imagen:</label>
            <input type="file" id="imagen" name="imagen" accepts="image/*" />
          </div>
          <div className="bottom-section">
            <button type="button" onClick={onClose}>
              Cancelar
            </button>
            <button type="submit" className="create-btn">Crear</button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default CategoryForm;
