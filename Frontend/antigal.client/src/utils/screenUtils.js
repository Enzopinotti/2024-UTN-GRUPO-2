// src/utils/screenUtils.js

/**
 * Retorna el número de productos visibles basado en el ancho de la pantalla.
 * @param {number} width - El ancho actual de la pantalla.
 * @returns {number} - El número de productos visibles.
 */
export const getVisibleItems = (width) => {
    if (width > 1700) return 6;  // Pantallas grandes
    if (width > 1080) return 4;  // Pantallas medianas
    if (width > 600) return 2;   // Tablets
    return 1;                    // Mobile
  };
  