import React from 'react';
import PropTypes from 'prop-types';
import formatCamelCase from '../../utils/formatCamelCase';

const CategoryList = ({ categories, onCategoryClick, selectedCategory }) => {
  return (
    <div className="category-list">
      <h3>Categorías</h3>
      <ul>
        {categories.map((category) => (
          <React.Fragment key={category.id}> {/* Cambiar a key={category.id} */}
            <li
              className={category.id === selectedCategory ? 'active' : ''} // Comparar con id
              onClick={() => onCategoryClick(category.id)} // Pasar el id
            >
              <span>{formatCamelCase(category.name)}</span>
              <span>({category.count})</span>
            </li>
            <hr />
          </React.Fragment>
        ))}
      </ul>
    </div>
  );
};

CategoryList.propTypes = {
  categories: PropTypes.arrayOf(
    PropTypes.shape({
      id: PropTypes.number.isRequired, // Asegúrate de incluir el id en la validación
      name: PropTypes.string.isRequired,
      count: PropTypes.number.isRequired,
    })
  ).isRequired,
  onCategoryClick: PropTypes.func.isRequired,
  selectedCategory: PropTypes.number, // Cambiar a number
};

export default CategoryList;