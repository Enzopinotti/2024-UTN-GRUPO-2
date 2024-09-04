import React, { useState, useEffect } from 'react';
import PrimeraSection from '../home/PrimeraSection';
import TerceraSection from '../home/TerceraSection';

const Home = () => {
  const [backgroundImage, setBackgroundImage] = useState('/images/fondoVerde.png');

  useEffect(() => {
    const updateBackgroundImage = () => {
      if (window.innerWidth > 1080) {
        setBackgroundImage('/images/fondoVerdeDesktop.png');
      } else {
        setBackgroundImage('/images/fondoVerde.png');
      }
    };

    // Ejecutar al cargar el componente para establecer la imagen correcta
    updateBackgroundImage();

    // Escuchar el cambio de tamaño de pantalla
    window.addEventListener('resize', updateBackgroundImage);

    // Cleanup: remover el listener cuando el componente se desmonte
    return () => {
      window.removeEventListener('resize', updateBackgroundImage);
    };
  }, []);
  return (
    <div className='home_container'>
      <PrimeraSection />
      <TerceraSection backgroundImage={backgroundImage} />
    </div>
  );
};

export default Home;