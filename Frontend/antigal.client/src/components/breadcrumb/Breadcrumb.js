import React from 'react';
import { Link, useLocation } from 'react-router-dom';

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

          return isLast ? (
            <><span> &gt;</span><span key={to} className="active">  {value}</span></> // Último segmento activo en naranja
          ) : (
            <><span> &gt;</span><span key={to}> &gt; <Link to={to}>{value}</Link></span></>
          );
        })}
      </p>
    </div>
  );
};

export default Breadcrumb;
