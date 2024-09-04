import React from 'react';

const Home = () => {
  return (
    <div className='home_container'>
      <section className='primeraSection'>
        <img className='hojasUno' src="/images/imgHojas2.png" alt="Imagen de hojas de Antigal" />
        <img className='hojasDos' src="/images/hojasBlur.png" alt="Imagen de hojas secos de Antigal" />
        <h2 className='subtitulo'>SaludableMente Natural</h2>       
        <div className='contenedorFrasco'>
          <img className='imagenFrasco' src="/images/imgFrasco.png" alt="Imagen de frutos secos de Antigal" />
        </div>
        <div className='botonCompraContainer'>
          <button className='botonComprar'>Comprar</button>
        </div>
     
        <h1 className='titulo' >LA SELECCIÓN DE FRUTOS SECOS PERFECTA, EXISTE.</h1>
      </section>
      <section className='segundaSection'>
        <div className='contenedorFondo'>
          <img className='fondoVerde' src="/images/fondoVerde.png" alt="Imagen de fondo de Antigal" />
          
          <article className='contenedorFrutosSecos'>
            <div className='almendrasContenedor'>
              <article className='contenedorIconos'>
                <div className='circuloVerdeOscuro'></div>
                <img className='icono' src="/icons/almendra.png" alt="Imagen de fondo de Antigal" />
                
              </article>
              <h4 className='titulo'>ALMENDRAS</h4>
              <p className='parrafo'>Las almendras son ricas en nutrientes esenciales, incluyendo vitamina E, fibra y proteínas. Son un complemento perfecto para una dieta balanceada.</p>
            </div>
            <div className='pistachosContenedor'>
              <article className='contenedorIconos'>
                <div className='circuloVerdeOscuro'></div>
                <img className='icono' src="/icons/pistacho.png" alt="Imagen de fondo de Antigal" />
              </article>
              <h4 className='titulo'>PISTACHOS</h4>
              <p className='parrafo'>Los pistachos son una excelente fuente de antioxidantes, vitaminas y minerales. Son ideales como snack saludable y también se pueden agregar a una variedad de platos.</p>
            </div>
            <div className='maniContenedor'>
              <article className='contenedorIconos'>
                <div className='circuloVerdeOscuro'></div>
                <img className='icono' src="/icons/mani.png" alt="Imagen de fondo de Antigal" />
              </article>
              <h4 className='titulo'>MANÍ ORGANICO</h4>
              <p className='parrafo'>El maní orgánico es una fuente rica en proteínas y grasas saludables. Es perfecto para preparar mantequillas, agregar a ensaladas o simplemente disfrutar como un snack natural y nutritivo.</p>
            </div>
            <div className='energiaContenedor'>
              <article className='contenedorIconos'>
                <div className='circuloVerdeOscuro'></div>
                <img className='icono' src="/icons/rayo.png" alt="Imagen de fondo de Antigal" />
              </article>
              <h4 className='titulo'>100% DE ENERGÍA</h4>
              <p className='parrafo'>Obtén un impulso de energía natural con nuestros productos. Están diseñados para proporcionarte la vitalidad que necesitas para enfrentar el día con fuerza y entusiasmo, sin comprometer tu salud.</p>
            </div>
            
          </article>
        </div>
        <img className='fotoFrutos' src="/images/foto_frutos_secos.jpg" alt="Imagen de frutos secos de Antigal" />
      </section>
    </div>
  );
};

export default Home;