// src/components/admin/products/AdminProductListContainer.js
import React, { useState, useEffect } from 'react';
import AdminNav from '../AdminNav';
import ProductList from './ProductList';
import ProductForm from './ProductForm';
import Swal from 'sweetalert2';
import initialProducts from '../../../data/initialProducts'; // Importamos los datos iniciales

const AdminProductListContainer = () => {
  const [products, setProducts] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [editingProduct, setEditingProduct] = useState(null);

  // Variable para controlar el uso del backend
  const useBackend = false; // Cambia a 'true' para usar el backend

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        if (useBackend) {
          // Obtener productos desde el backend
          const response = await fetch('http://localhost:5000/productos');
          if (!response.ok) {
            throw new Error('Error al obtener productos del backend');
          }
          const data = await response.json();
          setProducts(data);
        } else {
          // Cargar productos desde localStorage o desde los datos iniciales
          const storedProducts = localStorage.getItem('products');
          if (storedProducts) {
            setProducts(JSON.parse(storedProducts));
          } else {
            setProducts(initialProducts);
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
  const handleAddProduct = async (product) => {
    try {
      if (useBackend) {
        // ... (código para el backend)
      } else {
        const newProducts = [...products, product];
        setProducts(newProducts);
        updateLocalStorage(newProducts);
        Swal.fire('¡Éxito!', 'Producto agregado correctamente.', 'success');
      }
    } catch (error) {
      console.error('Error al agregar producto:', error);
      Swal.fire('Error', 'No se pudo agregar el producto.', 'error');
    }
  };

  // Función para editar un producto existente
  const handleEditProduct = async (updatedProduct) => {
    try {
      if (useBackend) {
        // ... (código para el backend)
      } else {
        const newProducts = products.map((prod) =>
          prod.idProducto === updatedProduct.idProducto ? updatedProduct : prod
        );
        setProducts(newProducts);
        updateLocalStorage(newProducts);
        Swal.fire('¡Éxito!', 'Producto actualizado correctamente.', 'success');
      }
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
          if (useBackend) {
            // ... (código para el backend)
          } else {
            const newProducts = products.filter((prod) => prod.idProducto !== idProducto);
            setProducts(newProducts);
            updateLocalStorage(newProducts);
            Swal.fire('Eliminado', 'El producto ha sido eliminado.', 'success');
          }
        } catch (error) {
          console.error('Error al eliminar producto:', error);
          Swal.fire('Error', 'No se pudo eliminar el producto.', 'error');
        }
      }
    });
  };

  return (
    <div className="admi-page">
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
