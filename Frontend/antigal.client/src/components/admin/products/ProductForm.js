// src/components/admin/products/ProductForm.js
import React, { useState, useEffect } from 'react';
import Swal from 'sweetalert2';
import initialCategories from '../../../data/initialCategories'; // Importamos los datos iniciales

const ProductForm = ({ show, onClose, onSave, product }) => {
  // Estados para los campos del formulario
  const [nombre, setNombre] = useState('');
  const [precio, setPrecio] = useState('');
  const [categoria, setCategoria] = useState('');
  const [imagen, setImagen] = useState('');
  const [descripcion, setDescripcion] = useState('');
  const [codigoBarras, setCodigoBarras] = useState('');
  const [marca, setMarca] = useState('');
  const [stock, setStock] = useState('');
  const [disponible, setDisponible] = useState(true);

  // Estado para las categorías
  const [categorias, setCategorias] = useState([]);

  useEffect(() => {
    // Obtener categorías desde localStorage o desde los datos iniciales
    const storedCategories = localStorage.getItem('categories');
    if (storedCategories) {
      setCategorias(JSON.parse(storedCategories));
    } else {
      setCategorias(initialCategories);
    }
  }, []);

  // Efecto para cargar los datos del producto en caso de edición
  useEffect(() => {
    if (product) {
      setNombre(product.nombre);
      setPrecio(product.precio);
      setCategoria(product.categoria);
      setImagen(product.imagen || '');
      setDescripcion(product.descripcion);
      setCodigoBarras(product.codigoBarras);
      setMarca(product.marca);
      setStock(product.stock);
      setDisponible(product.disponible);
    } else {
      // Limpiar los campos si es un nuevo producto
      setNombre('');
      setPrecio('');
      setCategoria('');
      setImagen('');
      setDescripcion('');
      setCodigoBarras('');
      setMarca('');
      setStock('');
      setDisponible(true);
    }
  }, [product]);

  // Manejador para el cambio de imagen
  const handleImageChange = (e) => {
    setImagen(e.target.value);
  };

  // Validaciones del formulario
  const validateForm = () => {
    if (
      nombre.trim() === '' ||
      precio === '' ||
      categoria === '' ||
      descripcion.trim() === '' ||
      codigoBarras === '' ||
      marca.trim() === '' ||
      stock === ''
    ) {
      Swal.fire('Error', 'Todos los campos son obligatorios.', 'error');
      return false;
    }

    if (isNaN(precio) || parseFloat(precio) <= 0) {
      Swal.fire('Error', 'El precio debe ser un número positivo.', 'error');
      return false;
    }

    if (isNaN(stock) || parseInt(stock) < 0) {
      Swal.fire('Error', 'El stock debe ser un número entero no negativo.', 'error');
      return false;
    }

    if (!/^\d+$/.test(codigoBarras)) {
      Swal.fire('Error', 'El código de barras debe contener solo números.', 'error');
      return false;
    }

    return true;
  };

  // Manejador de envío del formulario
  const handleSubmit = (e) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    const newProduct = {
      idProducto: product ? product.idProducto : Date.now(),
      nombre,
      precio: parseFloat(precio),
      categoria,
      imagen,
      descripcion,
      codigoBarras,
      marca,
      stock: parseInt(stock),
      disponible,
    };

    onSave(newProduct);
    onClose();
  };

  // Manejadores para cerrar el modal al hacer clic fuera o en la "X"
  const handleOverlayClick = (event) => {
    if (event.target.className === 'form-overlay') {
      onClose();
    }
  };

  const handleFormClick = (event) => {
    event.stopPropagation();
  };

  // Mover la condición después de los Hooks
  if (!show) {
    return null;
  }

  return (
    <div className="form-overlay" onClick={handleOverlayClick}>
      <div className="form" onClick={handleFormClick}>
        <div className="top-section">
          <h2>{product ? 'Editar Producto' : 'Crear Nuevo Producto'}</h2>
          <button type="button" className="exit-btn" onClick={onClose}>
            &times;
          </button>
        </div>
        <form onSubmit={handleSubmit}>
          <div className="form-fields">
            <div className="left-column">
              <div className="input">
                <label htmlFor="nombre">Nombre del producto:</label>
                <input
                  type="text"
                  id="nombre"
                  name="nombre"
                  value={nombre}
                  onChange={(e) => setNombre(e.target.value)}
                  required
                />
              </div>
              <div className="input">
                <label htmlFor="precio">Precio:</label>
                <input
                  type="number"
                  id="precio"
                  name="precio"
                  value={precio}
                  onChange={(e) => setPrecio(e.target.value)}
                  min="0"
                  step="0.01"
                  required
                />
              </div>
              <div className="input">
                <label htmlFor="categoria">Categoría:</label>
                <select
                  id="categoria"
                  name="categoria"
                  value={categoria}
                  onChange={(e) => setCategoria(e.target.value)}
                  required
                >
                  <option value="">Seleccione una categoría</option>
                  {categorias.map((cat) => (
                    <option key={cat.idCategoria} value={cat.nombre}>
                      {cat.nombre}
                    </option>
                  ))}
                </select>
              </div>
              <div className="input">
                <label htmlFor="stock">Stock:</label>
                <input
                  type="number"
                  id="stock"
                  name="stock"
                  value={stock}
                  onChange={(e) => setStock(e.target.value)}
                  min="0"
                  required
                />
              </div>
              <div className="input">
                <label htmlFor="disponible">Disponible:</label>
                <select
                  id="disponible"
                  name="disponible"
                  value={disponible}
                  onChange={(e) => setDisponible(e.target.value === 'true')}
                  required
                >
                  <option value="true">Sí</option>
                  <option value="false">No</option>
                </select>
              </div>
            </div>
            <div className="right-column">
              <div className="input">
                <label htmlFor="codigoBarras">Código de Barras:</label>
                <input
                  type="text"
                  id="codigoBarras"
                  name="codigoBarras"
                  value={codigoBarras}
                  onChange={(e) => setCodigoBarras(e.target.value)}
                  required
                />
              </div>
              <div className="input">
                <label htmlFor="marca">Marca:</label>
                <input
                  type="text"
                  id="marca"
                  name="marca"
                  value={marca}
                  onChange={(e) => setMarca(e.target.value)}
                  required
                />
              </div>
              <div className="input">
                <label htmlFor="imagen">URL de la imagen:</label>
                <input
                  type="text"
                  id="imagen"
                  name="imagen"
                  value={imagen}
                  onChange={handleImageChange}
                />
              </div>
              <div className="input textarea">
                <label htmlFor="descripcion">Descripción:</label>
                <textarea
                  id="descripcion"
                  name="descripcion"
                  value={descripcion}
                  onChange={(e) => setDescripcion(e.target.value)}
                  required
                />
              </div>
            </div>
          </div>
          <div className="bottom-section">
            <button type="button" onClick={onClose}>
              Cancelar
            </button>
            <button type="submit" className="create-btn">
              {product ? 'Actualizar' : 'Crear'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ProductForm;
