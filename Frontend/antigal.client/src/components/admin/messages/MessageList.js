import { useState } from "react";
import React from "react";
import Swal from "sweetalert2";
const MessageList = ({ messages }) => {
  const [selectedMessage, setSelectedMessage] = useState(null);
  const [readMessages, setReadMessages] = useState([]);
  const handleClick = (message) => {
    setSelectedMessage(message);
    setReadMessages((prev) => [...prev, message.id]);
  };

  const handleCloseModal = () => {
    setSelectedMessage(null);
  };

  const handleSendReply = async () => {
    const respuesta = document.querySelector(".respuesta-input").value;

    if (!respuesta.trim()) {
      Swal.fire("Error", "La respuesta no puede estar vacía.", "error");
      return;
    }
    //modificar endpoint
    try {
      const response = await fetch(
        "https://localhost:7255/api/Contacto/sendReply",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            email: selectedMessage.email,
            subject: `Respuesta a: ${selectedMessage.asunto}`,
            message: respuesta,
          }),
        }
      );

      if (!response.ok) {
        throw new Error("Error al enviar el correo.");
      }

      Swal.fire("Enviado", "Tu respuesta ha sido enviada.", "success");
      handleCloseModal();
    } catch (error) {
      console.error("Error al enviar el correo:", error);
      Swal.fire("Error", "No se pudo enviar el correo.", "error");
    }
  };
  return (
    <div>
      {messages.length > 0 ? (
        messages.map((message) => (
          <div
            className={`message ${
              readMessages.includes(message.id) ? "read" : ""
            }`}
            key={message.id}
            onClick={() => handleClick(message)}
          >
            <div className="message-info name">
              <h3>{message.name}</h3>
            </div>

            <div className="message-info text">
              <p>
                <strong>{message.asunto}: </strong> 
                {message.mensaje.length > 100
                  ? `${message.mensaje.slice(0, 100)}...`
                  : message.mensaje}
              </p>
            </div>
            <div className="message-info date">
              {new Date(message.fecha).toLocaleDateString()}{" "}
              {new Date(message.fecha).toLocaleTimeString([], {
                hour: "2-digit",
                minute: "2-digit",
              })}
            </div>
          </div>
        ))
      ) : (
        <p>No hay mensajes para mostrar</p>
      )}

      {selectedMessage && (
        <div className="modal-overlay">
          <div className="modal">
            <h2>{selectedMessage.asunto}</h2>
            <p>
              <strong>De:</strong> {selectedMessage.name} (
              {selectedMessage.email})
            </p>
            <p>{selectedMessage.mensaje}</p>
            <textarea
              placeholder="Escribe tu respuesta aquí..."
              rows="4"
              style={{ width: "100%", marginTop: "10px" }}
            ></textarea>
            <div className="modal-buttons">
              <button onClick={handleSendReply}>Enviar Respuesta</button>
              <button onClick={handleCloseModal}>Cerrar</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default MessageList;
