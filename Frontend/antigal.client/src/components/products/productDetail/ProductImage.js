const ProductImage = ({ images, name }) => {
  if (!images || images.length === 0) {
    return <p>No hay imágenes disponibles para este producto.</p>;
  }

  return (
    <div className="detail-img">
      {images.map((image, index) => (
        <img key={index} src={image} alt={name} />
      ))}
    </div>
  );
};

export default ProductImage;