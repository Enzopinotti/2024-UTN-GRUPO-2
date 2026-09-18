import {
  addProduct,
  deleteProductById,
  getProducts,
  saveProducts,
  updateProduct,
} from './productUtils';

describe('product local persistence', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  test('returns an empty list when no products are stored', () => {
    expect(getProducts()).toEqual([]);
  });

  test('saves and restores products', () => {
    const products = [{ idProducto: 1, nombre: 'Avena', precio: 1200 }];
    saveProducts(products);
    expect(getProducts()).toEqual(products);
  });

  test('appends a product without losing existing entries', () => {
    saveProducts([{ idProducto: 1, nombre: 'Avena' }]);
    addProduct({ idProducto: 2, nombre: 'Granola' });
    expect(getProducts()).toEqual([
      { idProducto: 1, nombre: 'Avena' },
      { idProducto: 2, nombre: 'Granola' },
    ]);
  });

  test('updates only the matching product', () => {
    saveProducts([
      { idProducto: 1, nombre: 'Avena', stock: 3 },
      { idProducto: 2, nombre: 'Granola', stock: 4 },
    ]);
    updateProduct({ idProducto: 2, nombre: 'Granola premium', stock: 7 });
    expect(getProducts()).toEqual([
      { idProducto: 1, nombre: 'Avena', stock: 3 },
      { idProducto: 2, nombre: 'Granola premium', stock: 7 },
    ]);
  });

  test('deletes only the requested product', () => {
    saveProducts([
      { idProducto: 1, nombre: 'Avena' },
      { idProducto: 2, nombre: 'Granola' },
    ]);
    deleteProductById(2);
    expect(getProducts()).toEqual([{ idProducto: 1, nombre: 'Avena' }]);
  });
});
