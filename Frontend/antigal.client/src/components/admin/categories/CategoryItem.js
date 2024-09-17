import React from "react";

const CategoryItem = ({ category }) => {
  return (
    <div className="category-item">
      <div className="category-img">
        <img src={category.imageUrl} alt={category.name} />
      </div>
      <div className="category-info">
        <div className="top-section">
          <div className="title">
            <h2>{category.name}</h2>
          </div>
          <div className="category-button">
            <button className="delete-btn">
              <i className="fa-solid fa-trash"></i> <span>Eliminar</span>
            </button>
            <button className="mod-btn">
              <i className="fa-solid fa-pencil"></i> <span>Modificar</span>
            </button>
          </div>
        </div>

        <div className="bottom-section">
          <p>{category.description}</p>
        </div>
      </div>
    </div>
  );
};

export default CategoryItem;
