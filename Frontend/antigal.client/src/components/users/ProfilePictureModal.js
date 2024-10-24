import React, { useState } from "react";
import Swal from "sweetalert2";

const ProfilePictureModal = ({ isOpen, onClose }) => {
  const [file, setFile] = useState(null);

  const handleFileChange = (e) => {
    setFile(e.target.files[0]);
  };

  const handleUpload = () => {
    if (!file) {
      Swal.fire({
        icon: "error",
        title: "¡Error!",
        text: "Por favor, selecciona un archivo.",
        confirmButtonText: "Aceptar",
      });
      return;
    }
    ///////FALTA CONEXION CON BACK Y LOGICA DE GUARDADO
    onClose();
  };

  return (
    isOpen && (
      <div className="modal-overlay">
        <div className="modal-content">
          <div className="top">
            <h2>Subir nueva foto de perfil</h2>
            <button className="modal-close-button" onClick={onClose}>
              <img src="/icons/cruz.svg" alt="cerrar"></img>
            </button>
          </div>

          <input type="file" accept="image/*" onChange={handleFileChange} />
          <div className="bottom">
            <button onClick={handleUpload}>Subir</button>
            <button onClick={onClose}>Cancelar</button>
          </div>
        </div>
      </div>
    )
  );
};

export default ProfilePictureModal;
