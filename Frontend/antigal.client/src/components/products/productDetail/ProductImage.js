import React from "react";

const ProductImage = ({ imageUrl, name }) => {
  return (
    <div className="detail-img">
      <img src={imageUrl} alt={name} />
    </div>
  );
};

export default ProductImage;