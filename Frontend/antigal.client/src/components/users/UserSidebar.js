import React,{useState} from "react";
import { NavLink } from "react-router-dom";
import Swal from "sweetalert2";
const UserSidebar = ({user}) => {
  const showDevelopmentAlert = (event) => {
    event.preventDefault();
    Swal.fire({
      title: "Funcionalidad en Desarrollo",
      text: "Esta funcionalidad estará disponible pronto.",
      icon: "info",
      confirmButtonText: "Cerrar",
    });
  };
  
  const [isOpen, setIsOpen] = useState(false);
  const toggleSidebar = () => {
    setIsOpen(!isOpen);
  };

  return (
    <>
      <div className="mobile-sidebar" onClick={toggleSidebar}>
        <button >
          <img src="/icons/user-white.svg" alt="profile"/>
        </button>
      </div>

      <div className={`user-sidebar ${isOpen? 'open mobile' :''}`}>
        <div className="user-content">
          <div className="user-profile-img">
            <img src={user.picture} alt="foto de perfil" />
          </div>
          <div className="user-profile-text">
            <h2>{user.name}</h2>
            <p>#{user.user}</p>
          </div>
        </div>
        <div className="sidebar-list">
          <ul>
            <li>
              <NavLink
                to="/perfil"
                className={({ isActive }) => (isActive ? "active" : undefined)}
              >
                Mi Perfil
              </NavLink>
            </li>
            <li>
              <NavLink
                to="/pedidos"
                className={({ isActive }) => (isActive ? "active" : undefined)}
                onClick={showDevelopmentAlert}
              >
                Mis Pedidos
              </NavLink>
            </li>
            <li>
              <NavLink
                to="/direcciones"
                className={({ isActive }) => (isActive ? "active" : undefined)}
                onClick={showDevelopmentAlert}
              >
                Mis Direcciones
              </NavLink>
            </li>
            <li>
              <NavLink
                to="/account-security"
                className={({ isActive }) => (isActive ? "active" : undefined)}
                onClick={showDevelopmentAlert}
              >
                Seguridad
              </NavLink>
            </li>
          </ul>
        </div>
      </div>
      {isOpen && <div className="overlay" onClick={toggleSidebar}></div>}
    </>
  );
};
export default UserSidebar;
