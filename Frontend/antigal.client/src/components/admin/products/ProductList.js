import React from "react";
import AdminProductItem from "./ProductItem";

const productData = [
    {
      id: 1,
      name: "Manzana",
      price: 1.2,
      imageUrl: "/images/product/manzana.jpg",
      category: "Frutas",
      stock: 0,
      description: "Las manzanas son una excelente fuente de fibra y vitamina C. Este fruto crujiente y dulce es ideal para un snack saludable o para agregar a tus ensaladas y postres. ¡Disfruta del sabor fresco y natural de nuestras manzanas de alta calidad!",
    },
    {
      id: 2,
      name: "Lechuga",
      price: 0.8,
      imageUrl: "/images/lechuga.jpg",
      category: "Verduras",
      stock: 10,
        description:"La lechuga es una planta de hojas verdes, crujiente y refrescante, común en ensaladas. Es baja en calorías y rica en agua, aportando fibra y algunas vitaminas como la A y K.",
    },
    {
      id: 3,
      name: "Avena",
      price: 2.5,
      imageUrl: "/images/product/avena.jpg",
      category: "Cereales",
      stock: 10,
      description:  "Comience su día con la energía y la naturalidad que le ofrece la Avena Arrollada Instantánea Yin Yang. Este producto de 500 gramos es la elección perfecta para quienes buscan una opción saludable y rápida para sus desayunos o meriendas. Con un sabor tradicional que rememora los desayunos caseros, nuestra avena es ideal para paladares que valoran la calidad y la simplicidad.",
    },
  ];

const AdminProductList = () => {
  return (
    
      <div className="product-list">
        {productData.map((product) => (
          <AdminProductItem key={product.id} product={product} />
        ))}
      </div>
   
  );
};

export default AdminProductList;
