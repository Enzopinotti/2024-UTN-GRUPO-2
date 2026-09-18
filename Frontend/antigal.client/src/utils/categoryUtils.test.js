import {
  addCategory,
  deleteCategory,
  getCategories,
  saveCategories,
  updateCategory,
} from './categoryUtils';

describe('category local persistence', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  test('returns an empty list when no categories are stored', () => {
    expect(getCategories()).toEqual([]);
  });

  test('saves and restores categories', () => {
    const categories = [{ idCategoria: 1, nombre: 'Frutos secos' }];
    saveCategories(categories);
    expect(getCategories()).toEqual(categories);
  });

  test('appends a category without losing existing entries', () => {
    saveCategories([{ idCategoria: 1, nombre: 'Frutos secos' }]);
    addCategory({ idCategoria: 2, nombre: 'Cereales' });
    expect(getCategories()).toEqual([
      { idCategoria: 1, nombre: 'Frutos secos' },
      { idCategoria: 2, nombre: 'Cereales' },
    ]);
  });

  test('updates only the matching category', () => {
    saveCategories([
      { idCategoria: 1, nombre: 'Frutos secos' },
      { idCategoria: 2, nombre: 'Cereales' },
    ]);
    updateCategory({ idCategoria: 2, nombre: 'Cereales integrales' });
    expect(getCategories()).toEqual([
      { idCategoria: 1, nombre: 'Frutos secos' },
      { idCategoria: 2, nombre: 'Cereales integrales' },
    ]);
  });

  test('deletes only the requested category', () => {
    saveCategories([
      { idCategoria: 1, nombre: 'Frutos secos' },
      { idCategoria: 2, nombre: 'Cereales' },
    ]);
    deleteCategory(1);
    expect(getCategories()).toEqual([{ idCategoria: 2, nombre: 'Cereales' }]);
  });
});
