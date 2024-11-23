// src/components/admin/products/ProductForm.js

import React, { useState, useEffect, useRef } from 'react';
import Swal from 'sweetalert2';
import { useForm, Controller } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import { productSchema } from '../../../validations/validationSchemas';

const ProductForm = ({ show, onClose, onSave, product }) => {
<<<<<<< HEAD
  // Estados para los campos del formulario
  const [nombre, setNombre] = useState('');
  const [precio, setPrecio] = useState('');
  const [categoria, setCategoria] = useState('');
  const [imagenes, setImagenes] = useState([]); 
  const [descripcion, setDescripcion] = useState('');
  const [codigoBarras, setCodigoBarras] = useState('');
  const [marca, setMarca] = useState('');
  const [stock, setStock] = useState('');
  const [disponible, setDisponible] = useState(true);
  const [productoDestacado, setProductoDestacado] = useState('No'); 

  // Estado para las categorías
  const [categorias, setCategorias] = useState([]);

  // Estado para detectar cambios (solo para edición)
  const [initialData, setInitialData] = useState({});

=======
>>>>>>> FrontEnd
  // Ref para el input de imágenes
  const fileInputRef = useRef(null);

  // Inicializar react-hook-form
  const { register, handleSubmit, control, reset, formState: { errors, isSubmitting } } = useForm({
    resolver: yupResolver(productSchema),
    defaultValues: {
      nombre: '',
      precio: '',
      categoria: '',
      descripcion: '',
      codigoBarras: '',
      marca: '',
      stock: '',
      disponible: 1,
      destacado: 0,
      imagenes: [],
    },
  });

  useEffect(() => {
    if (product) {
<<<<<<< HEAD
      setNombre(product.nombre || '');
      setPrecio(product.precio !== undefined ? product.precio.toString() : '');
      setCategoria(product.categoria || '');
      setDescripcion(product.descripcion || '');
      setCodigoBarras(product.codigoBarras || '');
      setMarca(product.marca || '');
      setStock(product.stock !== undefined ? product.stock.toString() : '');
      setDisponible(product.disponible !== undefined ? product.disponible : true);
      setProductoDestacado(product.destacado || 'No');
      setImagenes(product.imagenes || []);
      
      // Guardar datos iniciales para detectar cambios
      setInitialData({
=======
      reset({
>>>>>>> FrontEnd
        nombre: product.nombre || '',
        precio: product.precio !== undefined ? product.precio : '',
        categoria: product.categoria || '',
        descripcion: product.descripcion || '',
        codigoBarras: product.codigoBarras || '',
        marca: product.marca || '',
<<<<<<< HEAD
        stock: product.stock !== undefined ? product.stock.toString() : '',
        disponible: product.disponible !== undefined ? product.disponible : true,
        destacado: product.destacado || 'No',
      });
    } else {
      setNombre('');
      setPrecio('');
      setCategoria('');
      setDescripcion('');
      setCodigoBarras('');
      setMarca('');
      setStock('');
      setDisponible(true);
      setProductoDestacado('No');
      setImagenes([]);
      setInitialData({});
=======
        stock: product.stock !== undefined ? product.stock : '',
        disponible: product.disponible !== undefined ? product.disponible : 1,
        destacado: product.destacado || 0,
        imagenes: [], // Las imágenes existentes no se manejan aquí
      });
    } else {
      reset({
        nombre: '',
        precio: '',
        categoria: '',
        descripcion: '',
        codigoBarras: '',
        marca: '',
        stock: '',
        disponible: 1,
        destacado: 0,
        imagenes: [],
      });
>>>>>>> FrontEnd
    }
  }, [product, reset]);

  // Validaciones del formulario se manejan a través de react-hook-form y Yup

  // Manejador de envío del formulario
<<<<<<< HEAD
  const handleSubmit = (e) => {
    e.preventDefault();
=======
  const onSubmit = async (data) => {
    try {
      // Mostrar en consola los datos recibidos
      console.log('Datos del formulario de producto recibidos:', data);
>>>>>>> FrontEnd

      // Crear un FormData para enviar archivos y datos
      const formData = new FormData();

      if (product) {
        // Para edición, enviar idProducto y campos modificados
        const modifiedFields = {};

        // Comparar campos y agregar solo los modificados
        if (product.nombre !== data.nombre) modifiedFields.nombre = data.nombre;
        if (product.precio !== Number(data.precio)) modifiedFields.precio = Number(data.precio);
        if (product.categoria !== data.categoria) modifiedFields.categoria = data.categoria;
        if (product.descripcion !== data.descripcion) modifiedFields.descripcion = data.descripcion;
        if (product.codigoBarras !== data.codigoBarras) modifiedFields.codigoBarras = data.codigoBarras;
        if (product.marca !== data.marca) modifiedFields.marca = data.marca;
        if (product.stock !== Number(data.stock)) modifiedFields.stock = Number(data.stock);
        if (product.disponible !== Number(data.disponible)) modifiedFields.disponible = Number(data.disponible);
        if (product.destacado !== Number(data.destacado)) modifiedFields.destacado = Number(data.destacado);

        if (Object.keys(modifiedFields).length > 0 || data.imagenes.length > 0) {
          formData.append('idProducto', product.idProducto); // Asegúrate de enviar el ID del producto
          formData.append('fields', JSON.stringify(modifiedFields));
        } else {
          Swal.fire('Información', 'No hay cambios para guardar.', 'info');
          return;
        }
      } else {
        // Para creación, enviar todos los campos relevantes
        const newProduct = {
          nombre: data.nombre,
          precio: Number(data.precio),
          categoria: data.categoria,
          descripcion: data.descripcion,
          codigoBarras: data.codigoBarras,
          marca: data.marca,
          stock: Number(data.stock),
          disponible: Number(data.disponible),
          destacado: Number(data.destacado),
        };

        formData.append('producto', JSON.stringify(newProduct));
      }

      // Añadir cada imagen al FormData
      if (data.imagenes && data.imagenes.length > 0) {
        Array.from(data.imagenes).forEach((imagen) => {
          formData.append('imagenes', imagen);
        });
      }

      // Mostrar en consola el FormData
      console.log('FormData a enviar:', formData);

<<<<<<< HEAD
      // Crear FormData para enviar archivos y datos
      formData.append('producto', JSON.stringify(newProduct));
    }

    // Añadir cada imagen al FormData
    imagenes.forEach((imagen) => {
      formData.append('imagenes', imagen);
    });

    onSave(formData);
    onClose();

    // Reset file input
    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
  };

  // Manejador para seleccionar imágenes
  const handleImageChange = (e) => {
    const files = Array.from(e.target.files);
    if (files.length + imagenes.length > 6) {
      Swal.fire('Error', 'No puedes subir más de 6 imágenes por producto.', 'error');
      return;
    }
    setImagenes([...imagenes, ...files]);

    // Reset file input
    if (fileInputRef.current) {
      fileInputRef.current.value = '';
=======
      // Llamar a la función onSave pasada desde el padre
      await onSave(formData);
    } catch (error) {
      console.error('Error al procesar el formulario de producto:', error);
      Swal.fire('Error', 'No se pudo procesar el formulario de producto.', 'error');
>>>>>>> FrontEnd
    }
  };

  // Manejador para eliminar una imagen seleccionada
  const handleRemoveImage = (index, imageList, setValue) => {
    const updatedImages = Array.from(imageList);
    updatedImages.splice(index, 1);
    setValue('imagenes', updatedImages);
  };

  // Manejadores para cerrar el modal al hacer clic fuera o en la "X"
  const handleOverlayClick = (event) => {
    if (event.target.classList.contains('form-overlay')) {
      onClose();
    }
  };

  const handleFormClick = (event) => {
    event.stopPropagation();
  };

  // Condición para mostrar o no el formulario
  if (!show) {
    return null;
  }

  return (
    <div className="form-overlay" onClick={handleOverlayClick}>
      <div className="form-container" onClick={handleFormClick}>
        <div className="form-header">
          <h2>{product ? 'Editar Producto' : 'Crear Nuevo Producto'}</h2>
          <button type="button" className="close-button" onClick={onClose}>
            &times;
          </button>
        </div>
        <form className="product-form" onSubmit={handleSubmit(onSubmit)}>
          {/* Primera fila: Nombre y Descripción */}
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="nombre">
                Nombre del producto<span className="required">*</span>:
              </label>
              <input
                type="text"
                id="nombre"
                {...register('nombre')}
                placeholder="Ingresa el nombre del producto"
                autoComplete="off"
              />
              {errors.nombre && <p className="error-message">{errors.nombre.message}</p>}
            </div>

            <div className="form-group">
              <label htmlFor="descripcion">
                Descripción<span className="required">*</span>:
              </label>
              <textarea
                id="descripcion"
                {...register('descripcion')}
                placeholder="Ingresa la descripción del producto"
                autoComplete="off"
              />
              {errors.descripcion && <p className="error-message">{errors.descripcion.message}</p>}
            </div>
          </div>

          {/* Segunda fila: Precio y Stock */}
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="precio">
                Precio<span className="required">*</span>:
              </label>
              <input
                type="number"
                id="precio"
                {...register('precio')}
                placeholder="Ingresa el precio del producto"
                step="0.01"
                min="0"
              />
              {errors.precio && <p className="error-message">{errors.precio.message}</p>}
            </div>

            <div className="form-group">
              <label htmlFor="stock">
                Stock<span className="required">*</span>:
              </label>
              <input
                type="number"
                id="stock"
                {...register('stock')}
                placeholder="Ingresa el stock del producto"
                step="1"
                min="0"
              />
              {errors.stock && <p className="error-message">{errors.stock.message}</p>}
            </div>
          </div>

          {/* Tercera fila: Categoría y Destacado */}
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="categoria">Categoría<span className="required">*</span>:</label>
              <select
                id="categoria"
                {...register('categoria')}
                defaultValue=""
              >
                <option value="" disabled>
                  Seleccione una categoría
                </option>
                {/* Asegúrate de que las categorías se pasen correctamente */}
                {/* Puedes mapear desde props o desde el estado */}
                {/* Aquí asumimos que tienes una lista de categorías en algún lugar */}
                {/* Ejemplo estático: */}
                <option value="Electrónica">Electrónica</option>
                <option value="Ropa">Ropa</option>
                <option value="Hogar">Hogar</option>
                <option value="Juguetes">Juguetes</option>
                {/* ... otras categorías */}
              </select>
              {errors.categoria && <p className="error-message">{errors.categoria.message}</p>}
            </div>

            <div className="form-group">
              <label>Destacado<span className="required">*</span>:</label>
              <div className="radio-options">
                <div className="radio-option">
                  <input
                    type="radio"
<<<<<<< HEAD
                    id="destacadoHome"
                    name="destacado"
                    value="Home"
                    checked={productoDestacado === 'Home'}
                    onChange={(e) => setProductoDestacado(e.target.value)}
=======
                    id="destacadoSi"
                    value={1}
                    {...register('destacado')}
                    defaultChecked={!product || product.destacado === 1}
>>>>>>> FrontEnd
                  />
                  <label htmlFor="destacadoSi">Sí</label>
                </div>
                <div className="radio-option">
                  <input
                    type="radio"
                    id="destacadoNo"
<<<<<<< HEAD
                    name="destacado"
                    value="No"
                    checked={productoDestacado === 'No'}
                    onChange={(e) => setProductoDestacado(e.target.value)}
=======
                    value={0}
                    {...register('destacado')}
                    defaultChecked={product && product.destacado === 0}
>>>>>>> FrontEnd
                  />
                  <label htmlFor="destacadoNo">No</label>
                </div>
              </div>
              {errors.destacado && <p className="error-message">{errors.destacado.message}</p>}
            </div>
          </div>

          {/* Cuarta fila: Marca y Código de Barra */}
          <div className="form-row">
            <div className="form-group">
              <label htmlFor="marca">Marca:</label>
              <input
                type="text"
                id="marca"
                {...register('marca')}
                placeholder="Ingresa la marca del producto"
                autoComplete="off"
              />
              {errors.marca && <p className="error-message">{errors.marca.message}</p>}
            </div>

            <div className="form-group">
              <label htmlFor="codigoBarras">Código de Barra:</label>
              <input
                type="text"
                id="codigoBarras"
                {...register('codigoBarras')}
                placeholder="Ingresa el código de barra del producto"
                autoComplete="off"
              />
              {errors.codigoBarras && <p className="error-message">{errors.codigoBarras.message}</p>}
            </div>
          </div>

          {/* Quinta fila: Subir Imágenes */}
          <div className="form-group">
            <label htmlFor="imagenes">Subir imágenes (máximo 6):</label>
            <input
              type="file"
              id="imagenes"
              {...register('imagenes')}
              accept="image/*"
              multiple
              ref={fileInputRef}
            />
            {errors.imagenes && <p className="error-message">{errors.imagenes.message}</p>}
          </div>

          {/* Mostrar vistas previas de las imágenes seleccionadas */}
<<<<<<< HEAD
          {imagenes.length > 0 && (
            <div className="image-previews">
              {imagenes.map((imagen, index) => {
                const isFile = typeof imagen !== 'string';
                const src = isFile ? URL.createObjectURL(imagen) : imagen;
                const key = isFile ? `${imagen.name}-${index}` : `existing-${index}`;
                
                return (
                  <div key={key} className="image-preview">
                    <img
                      src={src}
                      alt={`Imagen ${index + 1}`}
                      onLoad={() => {
                        if (isFile) {
                          URL.revokeObjectURL(src); // Liberar el objeto URL después de cargar la imagen
                        }
                      }}
                    />
                    <button
                      type="button"
                      className="remove-image-button"
                      onClick={() => handleRemoveImage(index)}
                    >
                      &times;
                    </button>
                  </div>
                );
              })}
            </div>
          )}
=======
          <Controller
            control={control}
            name="imagenes"
            render={({ field: { onChange, value } }) => (
              <div className="image-previews">
                {value && value.length > 0 && (
                  Array.from(value).map((imagen, index) => {
                    const src = URL.createObjectURL(imagen);
                    return (
                      <div key={index} className="image-preview">
                        <img
                          src={src}
                          alt={`Imagen ${index + 1}`}
                          onLoad={() => {
                            URL.revokeObjectURL(src); // Liberar el objeto URL después de cargar la imagen
                          }}
                        />
                        <button
                          type="button"
                          className="remove-image-button"
                          onClick={() => handleRemoveImage(index, value, onChange)}
                        >
                          &times;
                        </button>
                      </div>
                    );
                  })
                )}
              </div>
            )}
          />
>>>>>>> FrontEnd

          {/* Sección de Botones de Acción */}
          <div className="form-actions">
            <button type="button" className="cancel-button" onClick={onClose} disabled={isSubmitting}>
              Cancelar
            </button>
            <button type="submit" className="submit-button" disabled={isSubmitting}>
              {isSubmitting ? 'Procesando...' : (product ? 'Actualizar' : 'Crear')}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ProductForm;
<<<<<<< HEAD

=======
>>>>>>> FrontEnd
