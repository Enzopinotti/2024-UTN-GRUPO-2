// src/components/common/LoadingSVG.js
const LoadingSVG = () => {
  return (
    <div className="loading-svg">
      {/* Usamos el SVG desde la carpeta public */}
      <img src="/icons/loadingAnimation.svg" alt="Cargando..." />
    </div>
  );
};

export default LoadingSVG;
