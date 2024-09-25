import React, { useState, useEffect } from 'react';
import ProductList from './ProductList';
import CategoryList from '../../categories/CategoryList';
import Breadcrumb from '../../breadcrumb/Breadcrumb';
import CartPreview from '../../carts/CartPreview'; // Importamos el CartPreview
import { useLocation } from 'react-router-dom';
import LoadingSVG from '../../common/LoadingSVG'; // Animación de carga
import ErrorAnimation from '../../common/ErrorAnimation'; // Animación de error

const ProductListContainer = () => {
  const [products, setProducts] = useState([]);
  const [filteredProducts, setFilteredProducts] = useState([]);
  const [categories, setCategories] = useState([]);  // Estado para almacenar las categorías con el formato {name, count}
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [loading, setLoading] = useState(true); // Estado de carga
  const [error, setError] = useState(false);    // Estado de error

  const location = useLocation();  // Para Breadcrumb dinámico

  useEffect(() => {
    const useBackend = false; // Cambia este flag a true para conectar con el backend
    const fetchURL = useBackend ? 'http://localhost:5000/api/products' : 'https://fakestoreapi.com/products';

    fetch(fetchURL)
      .then(response => {
        if (!response.ok) {
          throw new Error('Error al obtener productos');
        }
        return response.json();
      })
      .then(data => {
        const adaptedData = data.map(item => ({
          id: item.id,
          name: item.title,
          price: item.price,
          imageUrl: item.image,
          category: item.category,
        }));

        setProducts(adaptedData);
        setFilteredProducts(adaptedData); // Inicialmente todos los productos

        // Generar un objeto para contar productos por categoría
        const categoryCount = adaptedData.reduce((acc, product) => {
          acc[product.category] = (acc[product.category] || 0) + 1;
          return acc;
        }, {});

        // Formatear las categorías como {name, count} para el componente CategoryList
        const formattedCategories = Object.entries(categoryCount).map(([name, count]) => ({
          name,
          count,
        }));

        setCategories(formattedCategories);  // Actualizar el estado con las categorías formateadas
        setLoading(false); 
        setError(false); 
      })
      .catch(() => {
        setLoading(false);  // Ya no está cargando
        setError(true);     // Ha ocurrido un error
      });
  }, []);

  // Alternar el estado del dropdown de categorías en mobile
  const toggleDropdown = () => {
    setIsDropdownOpen(prevState => !prevState);  // Cambio a una función que toma el estado anterior
  };

  // Función para manejar el filtrado de categorías
  const handleCategoryClick = (categoryName) => {
    if (categoryName === selectedCategory) {
      setFilteredProducts(products); // Mostrar todos los productos si se deselecciona la categoría
      setSelectedCategory(null);
    } else {
      const filtered = products.filter(product => product.category === categoryName);
      setFilteredProducts(filtered);
      setSelectedCategory(categoryName); // Establecer la categoría seleccionada
    }
  };

  return (
    <div className="products-page">
      {/* En mobile, mostramos el botón de filtro y el breadcrumb */}
      <div className="mobile-filters">
        <button className="filter-button" onClick={toggleDropdown}>
          <img src="/icons/filterIcon.svg" alt="Filter" />
        </button>
        <Breadcrumb currentLocation={location.pathname} /> {/* Breadcrumb dinámico */}
      </div>

      {/* Dropdown de categorías en mobile */}
      {isDropdownOpen && (
        <div className="categories-dropdown open">
          <CategoryList
            categories={categories}
            onCategoryClick={handleCategoryClick}
            selectedCategory={selectedCategory}
          />
        </div>
      )}

      <div className='asideYProductos'>
        <aside className="categories-aside">
          <Breadcrumb currentLocation={location.pathname} />
          <CategoryList
            categories={categories}
            onCategoryClick={handleCategoryClick}
            selectedCategory={selectedCategory}
          />
        </aside>

        <div className="product-list-container">
          {loading ? (
            // Si está cargando, mostramos la animación de carga
            <div className="loading-container">
              <LoadingSVG />
            </div>
          ) : error ? (
            // Si hay error, mostramos la animación de error
            <div className="error-container">
              <ErrorAnimation />
              <h2>Oops... Algo salió mal. Intenta de nuevo más tarde.</h2>
            </div>
          ) : (
            <ProductList products={filteredProducts} />
          )}
        </div>

        <div className="cart-preview-container">
          <CartPreview />
        </div>
      </div>
    </div>
  );
};

export default ProductListContainer;
