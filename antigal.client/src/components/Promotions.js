import React from 'react';

const Promotions = () => {
  // Ejemplo de datos de promociones (puedes obtener estos datos desde una API más adelante)
  const promotions = [
    { id: 1, name: '50% de descuento en Frutas', description: 'Descuento en todas las frutas frescas.' },
    { id: 2, name: '3x2 en Cereales', description: 'Llévate 3 paquetes de cereales al precio de 2.' },
    // Agrega más promociones aquí
  ];

  return (
    <section>
      <h2>Promociones</h2>
      <ul>
        {promotions.map(promo => (
          <li key={promo.id}>
            <h3>{promo.name}</h3>
            <p>{promo.description}</p>
          </li>
        ))}
      </ul>
    </section>
  );
};

export default Promotions;