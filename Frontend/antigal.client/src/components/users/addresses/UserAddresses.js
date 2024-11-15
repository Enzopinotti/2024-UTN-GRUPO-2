import React, { useEffect, useState } from "react";
import { useOutletContext } from "react-router-dom";
import Swal from "sweetalert2";

const UserAddresses = () => {
  const { user: currentUser, setUserData } = useOutletContext();
  const [userData, setUserDataState] = useState(currentUser);
  const [editingAddress, setEditingAddress] = useState(null);
  const [newAddress, setNewAddress] = useState("");

  useEffect(() => {
    setUserDataState(currentUser);
  }, [currentUser]);

  const handleDeleteAddress = (addressId) => {
    Swal.fire({
      title: "¿Estás seguro?",
      text: "Esta acción no se puede deshacer.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#d33",
      cancelButtonColor: "#3085d6",
      confirmButtonText: "Sí, eliminar",
    }).then((result) => {
      if (result.isConfirmed) {
        const updatedAddresses = userData.direcciones.filter(
          (address) => address.id !== addressId
        );
        const updatedUserData = { ...userData, direcciones: updatedAddresses };
        setUserData(updatedUserData);
        setUserDataState(updatedUserData);

        Swal.fire(
          "Eliminado",
          "La dirección ha sido eliminada.",
          "success"
        );
      }
    });
  };

  const handleEditAddress = (address) => {
    setEditingAddress(address);
    setNewAddress(address.address);
  };

  const handleSaveEdit = () => {
    if (!newAddress.trim()) {
      Swal.fire({
        icon: "error",
        title: "¡Error!",
        text: "La dirección no puede estar vacía.",
      });
      return;
    }

    const updatedAddresses = userData.direcciones.map((address) =>
      address.id === editingAddress.id
        ? { ...address, address: newAddress }
        : address
    );
    const updatedUserData = { ...userData, direcciones: updatedAddresses };
    setUserData(updatedUserData);
    setUserDataState(updatedUserData);
    setEditingAddress(null);
    setNewAddress("");

    Swal.fire("¡Éxito!", "La dirección ha sido actualizada.", "success");
  };

  const handleCloseModal = () => {
    setEditingAddress(null);
    setNewAddress("");
  };

  return (
    <div>
      <div className="user-addresses">
        <h2>Direcciones guardadas de {userData.name}</h2>
        <div className="addresses-list">
          {userData.direcciones.length > 0 ? (
            userData.direcciones.map((address) => (
              <div className="address-item" key={address.id}>
                <p>{address.address}</p>
                <div className="btn-container">
                  <button
                    className="edit-btn"
                    onClick={() => handleEditAddress(address)}
                  >
                    Editar
                  </button>
                  <button
                    className="delete-btn"
                    onClick={() => handleDeleteAddress(address.id)}
                  >
                    Eliminar
                  </button>
                </div>
              </div>
            ))
          ) : (
            <p>No tienes direcciones guardadas.</p>
          )}
        </div>
  
      {/* Modal for editing an address */}
      {editingAddress && (
        <div className="modal-overlay">
          <div className="modal">
            <h3>Editar Dirección</h3>
            <input
              type="text"
              value={newAddress}
              onChange={(e) => setNewAddress(e.target.value)}
              placeholder="Nueva dirección"
            />
            <button onClick={handleSaveEdit}>Guardar cambios</button>
            <button onClick={handleCloseModal}>Cancelar</button>
          </div>
        </div>
      )}
    </div>
    </div>

  );
};

export default UserAddresses;
