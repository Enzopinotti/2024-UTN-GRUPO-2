import React, { useState, useEffect } from 'react';
import ProductList from './ProductList';
import CategoryList from '../../categories/CategoryList';
import Breadcrumb from '../../breadcrumb/Breadcrumb';
import { useLocation } from 'react-router-dom';
import CartPreview from '../../carts/CartPreview'; // Importamos el CartPreview para mostrar el carrito

const ProductListContainer = () => {
  const [products, setProducts] = useState([]);
  const [filteredProducts, setFilteredProducts] = useState([]);
  const [categories, setCategories] = useState([]);
  const [isDropdownOpen, setIsDropdownOpen] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState(null);

  const location = useLocation();  // Para Breadcrumb dinámico

  useEffect(() => {
    const useBackend = false; // Cambia este flag a true para conectar con el backend
    const fetchURL = useBackend ? 'http://localhost:5000/api/products' : 'https://fakestoreapi.com/products';

    fetch(fetchURL)
      .then(response => response.json())
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

        const categoriesCount = adaptedData.reduce((acc, product) => {
          acc[product.category] = (acc[product.category] || 0) + 1;
          return acc;
        }, {});

        const categoriesArray = Object.entries(categoriesCount).map(([name, count]) => ({
          name,
          count
        }));

        setCategories(categoriesArray);
      })
      .catch(error => console.error('Error fetching products:', error));
  }, []);

  // Alternar el estado del dropdown de categorías en mobile
  const toggleDropdown = () => {
    setIsDropdownOpen(prevState => !prevState);  // Cambio a una función que toma el estado anterior
  };

  // Función para manejar el filtrado de categorías
  const handleCategoryClick = (category) => {
    if (category === selectedCategory) {
      setFilteredProducts(products); // Mostrar todos los productos si se deselecciona la categoría
      setSelectedCategory(null);
    } else {
      const filtered = products.filter(product => product.category === category);
      setFilteredProducts(filtered);
      setSelectedCategory(category); // Establecer la categoría seleccionada
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

      {/* En desktop, mostramos el breadcrumb, las categorías y la vista previa del carrito */}
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
          <ProductList products={filteredProducts} />
        </div>

        <div className="cart-preview-container">
          {/* Aquí se muestra la vista previa del carrito */}
          <CartPreview />
        </div>
      </div>
    </div>
  );
};

export default ProductListContainer;
