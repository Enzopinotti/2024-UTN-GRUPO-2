import UserSidebar from "./UserSidebar";
import { Outlet } from "react-router-dom";
import { useAuth0 } from "@auth0/auth0-react";
import { useState } from "react";
const UserLayout = () => {
  const { user: authUser, isAuthenticated, isLoading } = useAuth0();
  //si no hay backend usa esto:
  const usingBackend = false;

  const fakeUser = {
    id: 1,
    user: "usuario1",
    name: "Lucas Martinez",
    picture: "/images/fake-user.jpg",
    email: "lucas@example.com", // Añadido: email
    fechaNacimiento: "01/01/1999", // Añadido: fecha de nacimiento
    telefono: "+1234567890", // Añadido: teléfono
    genero: "Masculino", // Añadido: género
    dni: "12345678", // Añadido: DNI
  };

  const [userData, setUserData] = useState(
    usingBackend && isAuthenticated ? authUser : fakeUser
  );

  if (isLoading) {
    return <div>Cargando...</div>;
  }

  return (
    <div className="user-layout">
      <UserSidebar user={userData} />
      <div className="content-container">
        <div
          className="user-background"
          style={{
            backgroundImage: "url('/images/user-background.png')",
            width: "100%",
            height: "auto",
            minHeight:"100vh",
            backgroundSize: "cover",
            backgroundPosition: "center",
          }}
        >
          <Outlet context={{ user: userData, setUser: setUserData }} />
        </div>
      </div>
    </div>
  );
};

export default UserLayout;