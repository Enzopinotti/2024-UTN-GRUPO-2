import React, { useState } from "react";
import CategoryList from "./CategoryList";
import AdminNav from "../AdminNav";
import CategoryForm from "./CategoryForm";

const CategoryListContainer = () => {
  const [showModal, setShowModal] = useState(false);
  const handleShowModal = () => {
    setShowModal(true);
  };
  const handleCloseModal = () => {
    setShowModal(false);
  };

  return (
    <div className="admi-page">
      <AdminNav />
      <div className="new-btn">
        <button onClick={handleShowModal}> + Nueva Categoría </button>
      </div>
      <CategoryForm show={showModal} onClose={handleCloseModal} />
      <CategoryList />
    </div>
  );
};
export default CategoryListContainer;
