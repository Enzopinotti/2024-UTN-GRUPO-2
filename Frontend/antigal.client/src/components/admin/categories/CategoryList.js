import React from "react";
import CategoryItem from "./CategoryItem";

const categoryData = [
  {
    id: 1,
    name: "Sin TACC",
    imageUrl: "../../../images/category/sin-tacc.jpg",
    description:
      "Productos especialmente seleccionados para personas con sensibilidad al gluten o celíacos. Disfruta de una variedad de alimentos sin TACC, desde pan y pastas hasta galletas y más.",
  },
  {
    id: 2,
    name: "Vegano",
    imageUrl: "/images/category/vegan.avif",
    description:
      "Descubre nuestra selección de productos veganos, ideales para aquellos que siguen una dieta basada en plantas. Ofrecemos desde alimentos frescos hasta snacks, todos libres de ingredientes de origen animal.",
  },
  {
    id: 3,
    name: "Cereales",
    imageUrl: "/images/category/cereals.jpg",
    description:
      "Explora nuestra gama de cereales, perfectos para un desayuno saludable o un snack rápido. Desde granolas y copos de avena hasta mezclas de semillas y frutos secos, todos ricos en nutrientes.",
  },
  {
    id: 4,
    name: "Suplementos",
    imageUrl: "/images/category/supplements.jpg",
    description:
      "Encuentra suplementos nutricionales que apoyen tu salud y bienestar. Desde vitaminas y minerales hasta suplementos específicos para el rendimiento deportivo, todos elaborados con ingredientes de alta calidad.",
  },
];

const CategoryList = () => {
  return (
    
      <div className="category-list">
        {categoryData.map((category) => (
          <CategoryItem key={category.id} category={category} />
        ))}
      </div>
   
  );
};

export default CategoryList;
