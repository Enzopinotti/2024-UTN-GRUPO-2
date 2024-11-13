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
  
        // Adaptar los datos según la API utilizada
        let adaptedData;
        if (useBackend) {
          if (data.data.$values && Array.isArray(data.data.$values)) {
            // Adaptar los datos de los productos
            adaptedData = data.data.$values.map(item => ({
              id: item.idProducto,           
              name: item.nombre,            
              brand: item.marca,             
              description: item.descripcion, 
              barcode: item.codigoBarras,   
              available: item.disponible,    
              featured: item.destacado,      
              price: item.precio,            
              stock: item.stock,             
              immages: (item.imagenUrls.$values && item.imagenUrls.$values.length > 0) 
              ? item.imagenUrls.$values[0] 
              : '/icons/por-defecto.svg',   
              categories: [],                
            }));
          } else {
            throw new Error('Formato de datos incorrecto');
          }
        } else {
          if (data && Array.isArray(data)) {
            // Adaptar los datos de los productos de fakestore
            adaptedData = data.map(item => ({
              id: item.id,                   
              name: item.title,             
              brand: 'Marca',               
              description: item.description,  
              barcode: '123',                
              available: true,              
              featured: false,              
              price: item.price,             
              stock: 10,                     
              images: item.image,            
              categories: [item.category],   
            }));
          } else {
            throw new Error('Formato de datos incorrecto');
          }
        }
  
        const productsWithCategories = await Promise.all(
          adaptedData.map(async (product) => {
            if (useBackend) {
              const fetchURLCategory = `https://localhost:7255/api/ProductCategory/categorias/${product.id}`;
              try {
                const responseCategory = await fetch(fetchURLCategory);
                if (!responseCategory.ok) {
                  throw new Error(`Error al obtener categorías para el producto ${product.id}`);
                }
                const dataCategory = await responseCategory.json();
  
                if (dataCategory.data.$values && Array.isArray(dataCategory.data.$values)) {
                  const formattedCategories = dataCategory.data.$values.map(cat => formatCamelCase(cat.nombre));
                  return { ...product, categories: formattedCategories };
                } else {
                  return { ...product, categories: [] };
                }
              } catch (errorCategory) {
                console.error(`Error al obtener categorías para el producto ${product.id}: `, errorCategory);
                return { ...product, categories: [] };
              }
            } else {
              return product; 
            }
          })
        );
  
        // Actualizar el estado de productos
        setProducts(productsWithCategories);
        setFilteredProducts(productsWithCategories); // Inicialmente todos los productos
  
        // Generar un objeto para contar productos por categoría
        const categoryCount = {};
        productsWithCategories.forEach(product => {
          product.categories.forEach(category => {
            categoryCount[category] = (categoryCount[category] || 0) + 1;
          });
        });
  
        // Formatear las categorías como {name, count} para el componente CategoryList
        const formattedCategories = Object.entries(categoryCount).map(([name, count]) => ({
          name,
          count,
        }));
  
        setCategories(formattedCategories); // Actualizar el estado con las categorías formateadas
        setLoading(false);
        setError(false);
      } catch (err) {
        console.error('Error al obtener productos:', err);
        setLoading(false); // Ya no está cargando
        setError(true);    // Ha ocurrido un error
      }
    };
  
    fetchProductsAndCategories();
  }, [filter]);

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