import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import ProductImage from "./ProductImage";
import ProductInfo from "./ProductInfo";
import ProductStockControl from "./ProductStockControl";
import ProductMoreInfo from "./ProductMoreInfo";

const ProductDetail = () => {
  const { id } = useParams();
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
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

    const selectedProduct = productData.find((p) => p.id === parseInt(id));
    setProduct(selectedProduct || null);
    setLoading(false);
  }, [id]);

  if (loading) {
    return <p>Cargando...</p>;
  }

  if (!product) {
    return <p>Producto no encontrado</p>;
  }

  return (
    <div className="productDetail">
    <div className="detail-sideBar"></div>
    <div className="detail-container">
      <ProductImage imageUrl={product.imageUrl} name={product.name} />
      <div className="detail-side">
        <ProductInfo product={product} />
        <ProductStockControl product={product} />
        <ProductMoreInfo />
      </div>
    </div>
  </div>
  );
};

export default ProductDetail;
