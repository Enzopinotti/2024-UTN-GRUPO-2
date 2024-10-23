// src/components/admin/products/AdminProductListContainer.js
import React, { useState, useEffect } from 'react';
import AdminNav from '../AdminNav';
import ProductList from './ProductList';
import ProductForm from './ProductForm';
import Swal from 'sweetalert2';
import initialProducts from '../../../data/initialProducts'; 

const AdminProductListContainer = () => {
  const [products, setProducts] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [editingProduct, setEditingProduct] = useState(null);

  const useBackend = true; 

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        if (useBackend) {
          // Obtener productos desde el backend
<<<<<<< HEAD
          const response = await fetch('https://localhost:7255/api/Product/getProducts');
=======
          console.log('Entre a useBackend');
          const response = await fetch('http://localhost:5279/api/Product/getProduct');
>>>>>>> origin/FrontEnd
          if (!response.ok) {
            throw new Error('Error al obtener productos del backend');
          }
          const data = await response.json();
          if(Array.isArray(data.data)){
            setProducts(data.data);       
          }
          } else {
          // Cargar productos desde localStorage o desde los datos iniciales
          const storedProducts = localStorage.getItem('products');
          if (storedProducts) {
            setProducts(JSON.parse(storedProducts));
          } else {
            setProducts(initialProducts);
            // Inicializar localStorage con initialProducts si es necesario
            localStorage.setItem('products', JSON.stringify(initialProducts));
          }
        }
      } catch (error) {
        console.error('Error al obtener productos:', error);
        Swal.fire('Error', 'No se pudieron obtener los productos.', 'error');
      }
    };

    fetchProducts();
  }, [useBackend]);

  const handleShowModal = () => {
    setShowModal(true);
  };
  const handleCloseModal = () => {
    setShowModal(false);
    setEditingProduct(null);
  };

  // Función para actualizar localStorage
  const updateLocalStorage = (updatedProducts) => {
    localStorage.setItem('products', JSON.stringify(updatedProducts));
  };

  // Función para agregar un nuevo producto
  const handleAddProduct = async (formData) => {
    try {
      // Extraer datos del FormData
      const producto = JSON.parse(formData.get('producto') || '{}');
      const images = formData.getAll('imagenes');

      const newProduct = {
        ...producto,
        idProducto: Date.now(), // Generar un ID único
        imagenes: images,
      };

      const newProducts = [...products, newProduct];
      setProducts(newProducts);
      updateLocalStorage(newProducts);
      Swal.fire('¡Éxito!', 'Producto agregado correctamente.', 'success');
    } catch (error) {
      console.error('Error al agregar producto:', error);
      Swal.fire('Error', 'No se pudo agregar el producto.', 'error');
    }
  };
  
  const handleEditProduct = async (formData) => {
    try {
      // Extraer datos del FormData
      const idProducto = parseInt(formData.get('idProducto'));
      const fields = JSON.parse(formData.get('fields') || '{}');
      const images = formData.getAll('imagenes');

      const updatedProduct = {
        idProducto,
        ...fields,
      };

      // Manejar imágenes adicionales si existen
      if (images.length > 0) {
        const existingProduct = products.find(prod => prod.idProducto === idProducto);
        updatedProduct.imagenes = existingProduct.imagenes
          ? [...existingProduct.imagenes, ...images]
          : images;
      }

      const newProducts = products.map((prod) =>
        prod.idProducto === idProducto ? { ...prod, ...updatedProduct } : prod
      );
      setProducts(newProducts);
      updateLocalStorage(newProducts);
      Swal.fire('¡Éxito!', 'Producto actualizado correctamente.', 'success');
    } catch (error) {
      console.error('Error al actualizar producto:', error);
      Swal.fire('Error', 'No se pudo actualizar el producto.', 'error');
    }
  };

  // Función para eliminar un producto
  const handleDeleteProduct = (idProducto) => {
    Swal.fire({
      title: '¿Estás seguro?',
      text: 'No podrás revertir esta acción.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sí, eliminar',
      cancelButtonText: 'Cancelar',
    }).then(async (result) => {
      if (result.isConfirmed) {
        try {
          const newProducts = products.filter((prod) => prod.idProducto !== idProducto);
          setProducts(newProducts);
          updateLocalStorage(newProducts);
          Swal.fire('Eliminado', 'El producto ha sido eliminado.', 'success');
        } catch (error) {
          console.error('Error al eliminar producto:', error);
          Swal.fire('Error', 'No se pudo eliminar el producto.', 'error');
        }
      }
    });
  };

  return (
    <div className="admin-page"> {/* Cambiado de 'admi-page' a 'admin-page' */}
      <AdminNav />
      <div className="content">
        <div className="new-btn">
          <button onClick={handleShowModal}>+ Nuevo Producto</button>
        </div>
        <ProductForm
          show={showModal}
          onClose={handleCloseModal}
          onSave={editingProduct ? handleEditProduct : handleAddProduct}
          product={editingProduct}
        />
        <ProductList
          products={products}
          onEdit={(product) => {
            setEditingProduct(product);
            setShowModal(true);
          }}
          onDelete={handleDeleteProduct}
        />
      </div>
    </div>
  );
};

export default AdminProductListContainer;
