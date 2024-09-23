import React from 'react';
import PropTypes from 'prop-types';

const CategoryList = ({ categories, onCategoryClick, selectedCategory }) => {
  return (
    <div className="category-list">
      <h3>Categorías</h3>
      <ul>
        {categories.map((category) => (
          <li
            key={category.name}
            className={category.name === selectedCategory ? 'active' : ''}
            onClick={() => onCategoryClick(category.name)}
          >
            <span>{category.name}</span> <span>({category.count})</span>
          </li>
        ))}
      </ul>
    </div>
  );
};

CategoryList.propTypes = {
  categories: PropTypes.arrayOf(
    PropTypes.shape({
      name: PropTypes.string.isRequired,
      count: PropTypes.number.isRequired,
    })
  ).isRequired,
  onCategoryClick: PropTypes.func.isRequired,
  selectedCategory: PropTypes.string,
};

export default CategoryList;
