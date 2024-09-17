import React from "react";

const CategoryDropdown = ({ categories, onConfirm }) => {
  return (
    <div className="category-dropdown">
      <h4>Categorías</h4>
      {categories.map((category) => (
        <label key={category}>
          <input type="checkbox" value={category} />
          {category}
        </label>
      ))}
      <div className="btn">
        <button className="confirm-btn">Confirmar</button>
      </div>
    </div>
  );
};

export default CategoryDropdown;
