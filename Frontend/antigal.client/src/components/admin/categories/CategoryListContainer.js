// src/components/admin/categories/CategoryListContainer.js
import React, { useState, useEffect } from 'react';
import AdminNav from '../AdminNav';
import CategoryList from './CategoryList';
import CategoryForm from './CategoryForm';
import Swal from 'sweetalert2';
import initialCategories from '../../../data/initialCategories'; // Importamos los datos iniciales

const CategoryListContainer = () => {
  const [categories, setCategories] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [editingCategory, setEditingCategory] = useState(null);

  // Variable para controlar el uso del backend
  const useBackend = false; // Cambia a 'true' para usar el backend

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        if (useBackend) {
          // Obtener categorías desde el backend
          const response = await fetch('http://localhost:5000/categorias');
          if (!response.ok) {
            throw new Error('Error al obtener categorías del backend');
          }
          const data = await response.json();
          setCategories(data);
        } else {
          // Cargar categorías desde localStorage o desde los datos iniciales
          const storedCategories = localStorage.getItem('categories');
          if (storedCategories) {
            setCategories(JSON.parse(storedCategories));
          } else {
            setCategories(initialCategories);
          }
        }
      } catch (error) {
        console.error('Error al obtener categorías:', error);
        Swal.fire('Error', 'No se pudieron obtener las categorías.', 'error');
      }
    };

    fetchCategories();
  }, [useBackend]);

  const handleShowModal = () => {
    setShowModal(true);
  };
  const handleCloseModal = () => {
    setShowModal(false);
    setEditingCategory(null);
  };

  // Función para actualizar localStorage
  const updateLocalStorage = (updatedCategories) => {
    localStorage.setItem('categories', JSON.stringify(updatedCategories));
  };

  // Función para agregar una nueva categoría
  const handleAddCategory = (category) => {
    if (useBackend) {
      // Enviar solicitud POST al backend
      // Aquí iría el código para el backend
    } else {
      // Agregar categoría al estado local y actualizar localStorage
      const newCategories = [...categories, category];
      setCategories(newCategories);
      updateLocalStorage(newCategories);
      setShowModal(false);
      Swal.fire('¡Éxito!', 'Categoría añadida correctamente.', 'success');
    }
  };

  // Función para editar una categoría existente
  const handleEditCategory = (updatedCategory) => {
    if (useBackend) {
      // Enviar solicitud PUT al backend
      // Aquí iría el código para el backend
    } else {
      // Actualizar categoría en el estado local y en localStorage
      const newCategories = categories.map((cat) =>
        cat.idCategoria === updatedCategory.idCategoria ? updatedCategory : cat
      );
      setCategories(newCategories);
      updateLocalStorage(newCategories);
      setShowModal(false);
      setEditingCategory(null);
      Swal.fire('¡Éxito!', 'Categoría actualizada correctamente.', 'success');
    }
  };

  // Función para eliminar una categoría
  const handleDeleteCategory = (idCategoria) => {
    Swal.fire({
      title: '¿Estás seguro?',
      text: 'No podrás revertir esta acción.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sí, eliminar',
      cancelButtonText: 'Cancelar',
    }).then((result) => {
      if (result.isConfirmed) {
        if (useBackend) {
          // Enviar solicitud DELETE al backend
          // Aquí iría el código para el backend
        } else {
          // Eliminar categoría del estado local y actualizar localStorage
          const newCategories = categories.filter((cat) => cat.idCategoria !== idCategoria);
          setCategories(newCategories);
          updateLocalStorage(newCategories);
          Swal.fire('Eliminado', 'La categoría ha sido eliminada.', 'success');
        }
      }
    });
  };

  return (
    <div className="admi-page">
      <AdminNav />
      <div className="content">
        <div className="new-btn">
          <button onClick={handleShowModal}>+ Nueva Categoría</button>
        </div>
        <CategoryForm
          show={showModal}
          onClose={handleCloseModal}
          onSave={editingCategory ? handleEditCategory : handleAddCategory}
          category={editingCategory}
        />
        <CategoryList
          categories={categories}
          onEdit={(category) => {
            setEditingCategory(category);
            setShowModal(true);
          }}
          onDelete={handleDeleteCategory}
        />
      </div>
    </div>
  );
};

export default CategoryListContainer;
