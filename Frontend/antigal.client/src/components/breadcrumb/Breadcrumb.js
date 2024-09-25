import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import formatCamelCase from '../../utils/formatCamelCase';  // Importamos la función

const Breadcrumb = () => {
  const location = useLocation();
  const pathnames = location.pathname.split('/').filter((item) => item); // Dividimos la ruta en segmentos

  return (
    <div className="breadcrumb">
      <p>
        <Link to="/">Inicio</Link>
        {pathnames.map((value, index) => {
          const to = `/${pathnames.slice(0, index + 1).join('/')}`;
          const isLast = index === pathnames.length - 1;

          // Aplicamos formatCamelCase al valor del segmento
          const formattedValue = formatCamelCase(value);

          return isLast ? (
            <><span> <img className='imgBread' src='/icons/flechaBreadgrumb.svg' /></span><span key={to} className="active">  {formattedValue}</span></> // Último segmento activo en naranja
          ) : (
            <><span> <img className='imgBread' src='/icons/flechaBreadgrumb.svg' /></span><span key={to}><Link to={to}>{formattedValue}</Link></span></>
          );
        })}
      </p>
    </div>
  );
};

export default Breadcrumb;
