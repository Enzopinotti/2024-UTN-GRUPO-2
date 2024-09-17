import React from "react";
import { NavLink } from "react-router-dom";

const AdminNav = () => {
  return (
    <div className="admi-nav">
      <h2> Administrador</h2>
      <div className="admi-btn">
      <NavLink 
          to="/admin/categories" 
          className={({ isActive }) => isActive ? 'nav-button active' : 'nav-button'}
        >
          Categorías
        </NavLink>
        <NavLink 
          to="/admin/products" 
          className={({ isActive }) => isActive ? 'nav-button active' : 'nav-button'}
        >
          Productos
        </NavLink>
      </div>
    </div>
  );
};
export default AdminNav;
