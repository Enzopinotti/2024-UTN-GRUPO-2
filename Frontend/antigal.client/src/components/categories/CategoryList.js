import React from 'react';
import PropTypes from 'prop-types';
import formatCamelCase from '../../utils/formatCamelCase';

const CategoryList = ({ categories, onCategoryClick, selectedCategory }) => {
  return (
    <div className="category-list">
      <h3>Categorías</h3>
      <ul>
        {categories.length > 0 ? (
          categories.map((category) => (
            <React.Fragment key={category.id}> {/* Cambiar a category.id */}
              <li
                className={category.name === selectedCategory ? 'active' : ''}
                onClick={() => onCategoryClick(category.name, category.id)}
              >
                <span>{formatCamelCase(category.name)}</span>
                <span>({category.count})</span>
              </li>
              <hr />
            </React.Fragment>
          ))
        ) : (
          <li>No hay categorías disponibles</li> // Mensaje si no hay categorías
        )}
      </ul>
    </div>
  );
};

CategoryList.propTypes = {
  categories: PropTypes.arrayOf(
    PropTypes.shape({
      name: PropTypes.string.isRequired,
      count: PropTypes.number.isRequired,
      id: PropTypes.number.isRequired,
    })
  ).isRequired,
  onCategoryClick: PropTypes.func.isRequired,
  selectedCategory: PropTypes.string,
};

export default CategoryList;