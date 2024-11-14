import React, { useState, useEffect } from 'react';
import ProductList from './ProductList';
import ProductFilter from './ProductFilter';
import CategoryList from '../../categories/CategoryList';
import Breadcrumb from '../../breadcrumb/Breadcrumb';
import CartPreview from '../../carts/CartPreview';
import { useLocation } from 'react-router-dom';
import LoadingSVG from '../../common/LoadingSVG';
import ErrorAnimation from '../../common/ErrorAnimation';
import Banner from '../../common/Banner';
import formatCamelCase from '../../../utils/formatCamelCase';

const ProductListContainer = () => {
  const [products, setProducts] = useState([]);
  const [filteredProducts, setFilteredProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(false);
  const [filter, setFilter] = useState('');
  const [categoriaId, setCategoriaId] = useState(null);

  const location = useLocation();

  useEffect(() => {
    const fetchProductsAndCategories = async () => {
      const useBackend = true;
      let fetchURL = useBackend
        ? 'https://localhost:7255/api/Product/getProducts'
        : 'https://fakestoreapi.com/products';
  
      const params = new URLSearchParams();
      if (filter) {
        if (filter === 'antiguos' || filter === 'recientes') {
          params.append('orden', filter);
        } else if (filter === 'precioAscendente' || filter === 'precioDescendente') {
          params.append('precio', filter === 'precioAscendente' ? 'ascendente' : 'descendente');
        }
      }

      if (categoriaId) {
        params.append('categoriaId', categoriaId);
      }

      fetchURL += `?${params.toString()}`;

      try {
        const response = await fetch(fetchURL);
        if (!response.ok) {
          throw new Error('Error al obtener productos');
        }
        const data = await response.json();
        console.log("la data es:  ", data);

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
            categories: item.categoriaProductos.$values.map(cat => cat.nombre) || [], // Inicializar categorías vacías
          }));

          // Contar las categorías
          const categoryCount = {};
          adaptedData.forEach(product => {
            product.categories.forEach(category => {
              const formattedCategory = formatCamelCase(category);
              categoryCount[formattedCategory] = (categoryCount[formattedCategory] || 0) + 1;
            });
          });

          const formattedCategories = Object.entries(categoryCount).map(([name, count]) => ({
            name,
            count,
            id: name, // Asegúrate de que el id sea único o usa un id real si está disponible
          }));

          setProducts(adaptedData);
          setFilteredProducts(adaptedData);
          setCategories(formattedCategories);  // Actualizar el estado con las categorías formateadas
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
  }, [filter, categoriaId]);

  const toggleDropdown = () => {
    setIsDropdownOpen(prevState => !prevState);
  };

  const handleCategoryClick = (categoryName, categoryId) => {
    if (categoryName === selectedCategory) {
      setFilteredProducts(products); // Mostrar todos los productos si se deselecciona la categoría
      setSelectedCategory(null);
      setCategoriaId(null); // Resetear categoriaId
    } else {
      const filtered = products.filter(product => 
        product.categories && product.categories.includes(categoryName)
      );
      setFilteredProducts(filtered);
      setSelectedCategory(categoryName);
      setCategoriaId(categoryId);
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
            <div className="loading-container">
              <LoadingSVG />
            </div>
          ) : error ? (
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