import React from "react";
import { NavLink } from "react-router-dom";

const Footer = () => {
  return (
    <footer>
      <div className="footer-container">
        <div className="footer-section description">
          <p>
            La mejor dietética de Ensenada, nuestro compromiso es con nuestros
            clientes que necesitan mejorar su salud y vitalidad.
          </p>
          <div className="social-media">
            <a href="#">
              {" "}
              <i className="fab fa-facebook"></i>{" "}
            </a>
            <a href="#">
              {" "}
              <i className="fab fa-instagram"></i>{" "}
            </a>
            <a href="#">
              {" "}
              <i className="fab fa-linkedin"></i>{" "}
            </a>
            <a href="#">
              {" "}
              <i className="fab fa-whatsapp"></i>{" "}
            </a>
          </div>
        </div>
        <div className="footer-section links">
          <h3>Nosotros</h3>
          <ul>
            <li>
              <NavLink to="/about" activeClassName="active">
                Sobre Nosotros
              </NavLink>
            </li>
            <li>
              <NavLink to="/store" activeClassName="active">
                Nuestra Tienda
              </NavLink>
            </li>
            <li>
              <NavLink to="/products" activeClassName="active">
                Nuestros Productos
              </NavLink>
            </li>
          </ul>
        </div>
        <div className="footer-section links">
          <h3>Soporte</h3>
          <ul>
            <li>
              <a href="#">FAQ</a>
            </li>
            <li>
              <NavLink to="/contact" activeClassName="active">
                Contacto
              </NavLink>
            </li>
          </ul>
        </div>
        <div className="footer-section contact">
          <h3>Contactos</h3>
          <p>Av. Bossinga 600-552, Ensenada, Buenos Aires 1925</p>
          <p>
            <i className="fas fa-phone"></i>&nbsp;&nbsp;+54 9 123456789
          </p>
          <p>
            <i className="fas fa-envelope"></i>&nbsp;&nbsp;antigal@email.com
          </p>
        </div>
      </div>
      <div className="footer-bottom">
        <p>&copy; {new Date().getFullYear()} Dietetica E-commerce</p>
        <img src="../../images/tarjetas.png" alt="tarjetas aceptadas"></img>
      </div>
    </footer>
  );
};

export default Footer;