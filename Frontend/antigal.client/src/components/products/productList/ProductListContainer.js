//ProductListContainer.js
import React, { useState, useEffect } from 'react';
import ProductList from './ProductList';
import ProductFilter from './ProductFilter';
import CategoryList from '../../categories/CategoryList';
import Breadcrumb from '../../breadcrumb/Breadcrumb';
import CartPreview from '../../carts/CartPreview'; // Importamos el CartPreview
import { useLocation } from 'react-router-dom';
import LoadingSVG from '../../common/LoadingSVG'; // Animación de carga
import ErrorAnimation from '../../common/ErrorAnimation'; // Animación de error
import Banner from '../../common/Banner';
import formatCamelCase from '../../../utils/formatCamelCase';

const ProductListContainer = () => {
  const [products, setProducts] = useState([]);
  const [filteredProducts, setFilteredProducts] = useState([]);
  const [categories, setCategories] = useState([]);  // Estado para almacenar las categorías con el formato {name, count}
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [loading, setLoading] = useState(true); // Estado de carga
  const [error, setError] = useState(false);    // Estado de error
  const [filter, setFilter] = useState('');

  const location = useLocation();  // Para Breadcrumb dinámico

  useEffect(() => {
    const fetchProductsAndCategories = async () => {
      const useBackend = true; // Cambia este flag a true para conectar con el backend
      let fetchURL = useBackend
        ? 'https://localhost:7255/api/Product/getProducts'
        : 'https://fakestoreapi.com/products';
  
      // Modificar la URL según el filtro seleccionado
      if (filter) {
        if (filter === 'antiguos' || filter === 'recientes') {
          fetchURL += `?orden=${filter}`;
        } else if (filter === 'precioAscendente' || filter === 'precioDescendente') {
          fetchURL += `?precio=${filter === 'precioAscendente' ? 'ascendente' : 'descendente'}`;
        }
      }

      try {
        // Primera llamada: Obtener productos
        const response = await fetch(fetchURL);
        if (!response.ok) {
          throw new Error('Error al obtener productos');
        }
        const data = await response.json();
        console.log("la data es:  ", data);

        if (data.data && data.data.$values && Array.isArray(data.data.$values)) {
          // Adaptar los datos de los productos
          const adaptedData = data.data.$values.map(item => ({
            id: item.idProducto,           // Mapear idProducto a id
            name: item.nombre,             // Mapear nombre a name
            brand: item.marca,             // Mapear marca a brand
            description: item.descripcion, // Mapear descripcion a description
            barcode: item.codigoBarras,    // Mapear codigoBarras a barcode
            available: item.disponible,    // Mapear disponible a available
            featured: item.destacado,      // Mapear destacado a featured
            price: item.precio,            // Mapear precio a price
            stock: item.stock,             // Mapear stock a stock
            images: item.imagenUrls.$values[0] || 'icons/por-defecto.png',         // Mapear imagenes a images
            categories: [],                // Inicializar categorías vacías
          }));

          setProducts(adaptedData);
          setFilteredProducts(adaptedData); // Inicialmente todos los productos

          // Obtener categorías
          const categoryResponse = await fetch('https://localhost:7255/api/Category/getCategories');
          if (!categoryResponse.ok) {
            throw new Error('Error al obtener categorías');
          }
          const categoryData = await categoryResponse.json();

          if (categoryData.data && categoryData.data.$values && Array.isArray(categoryData.data.$values)) {
            const formattedCategories = categoryData.data.$values.map(cat => ({
              id: cat.idCategoria, // Agregar el ID de la categoría
              name: cat.nombre,
              count: 0, // Inicialmente, el conteo es 0
            }));

            // Contar productos por categoría
            const categoryCount = {};
            adaptedData.forEach(product => {
              product.categories.forEach(category => {
                categoryCount[category] = (categoryCount[category] || 0) + 1;
              });
            });

            // Actualizar el conteo de las categorías
 formattedCategories.forEach(cat => {
              cat.count = categoryCount[cat.name] || 0; // Asignar el conteo correspondiente
            });

            setCategories(formattedCategories); // Actualizar el estado con las categorías formateadas
          } else {
            throw new Error('Formato de datos de categorías incorrecto');
          }

          setLoading(false);
          setError(false);
        } else {
          throw new Error('Formato de datos incorrecto');
        }
      } catch (err) {
        console.error('Error al obtener productos:', err);
        setLoading(false);
        setError(true);
      }
    };

    fetchProductsAndCategories();
  }, [filter]);

  // Alternar el estado del dropdown de categorías en mobile
  const toggleDropdown = () => {
    setIsDropdownOpen(prevState => !prevState);  // Cambio a una función que toma el estado anterior
  };

  // Función para manejar el filtrado de categorías
  const handleCategoryClick = async (categoryId) => {
    setSelectedCategory(categoryId);
    setLoading(true); // Iniciar carga

    try {
      const fetchURL = `https://localhost:7255/api/Product/getProducts?orden=recientes&categoriaId=${categoryId}`;
      const response = await fetch(fetchURL);
      if (!response.ok) {
        throw new Error('Error al obtener productos de la categoría');
      }
      const data = await response.json();

      if (data.data && data.data.$values && Array.isArray(data.data.$values)) {
        const adaptedData = data.data.$values.map(item => ({
          id: item.idProducto,
          name: item.nombre,
          brand: item.marca,
          description: item.descripcion,
          barcode: item.codigoBarras,
          available: item.disponible,
          featured: item.destacado,
          price: item.precio,
          stock: item.stock,
          images: item.imagenUrls.$values[0] || 'icons/por-defecto.png',
          categories: [],
        }));

        setFilteredProducts(adaptedData); // Actualizar productos filtrados
      } else {
        throw new Error('Formato de datos incorrecto');
      }
    } catch (err) {
      console.error('Error al obtener productos de la categoría:', err);
      setError(true);
    } finally {
      setLoading(false); // Finalizar carga
    }
  };

  const handleFilterChange = (selectedFilter) => {
    setFilter(selectedFilter);
  };

  return (
    <div className="products-page">
      
      <div className="mobile-filters">
        <button className="filter-button" onClick={toggleDropdown}>
          <img src="/icons/filterIcon.svg" alt="Filter" />
        </button>
        <Breadcrumb currentLocation={location.pathname} /> 
      </div>

      
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
            <>
              <Banner />
              <ProductFilter onFilterChange={handleFilterChange} />
              <ProductList products={filteredProducts} />
            </>
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