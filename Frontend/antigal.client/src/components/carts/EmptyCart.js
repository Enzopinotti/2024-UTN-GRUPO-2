// src/components/cart/EmptyCart.js
const EmptyCart = () => {
  return (
    <div className="empty-cart">
      <svg
        className="empty-cart-image"
        data-testid="empty-cart-animation"
        viewBox="0 0 192 192"
        role="img"
        aria-label="Carrito vacío"
      >
        <path
          d="M18 30h16l27 94h82l24-70H50"
          fill="none"
          stroke="#ED8B00"
          strokeWidth="12"
          strokeLinecap="round"
          strokeLinejoin="round"
          pathLength="1"
          strokeDasharray="1"
          strokeDashoffset="1"
        >
          <animate
            attributeName="stroke-dashoffset"
            values="1;0;0"
            keyTimes="0;0.7;1"
            dur="1.35s"
            repeatCount="indefinite"
          />
        </path>
        <circle cx="78" cy="148" r="9" fill="#ED8B00">
          <animate
            attributeName="opacity"
            values="0;0;1;1"
            keyTimes="0;0.55;0.7;1"
            dur="1.35s"
            repeatCount="indefinite"
          />
        </circle>
        <circle cx="137" cy="148" r="9" fill="#ED8B00">
          <animate
            attributeName="opacity"
            values="0;0;1;1"
            keyTimes="0;0.55;0.7;1"
            dur="1.35s"
            repeatCount="indefinite"
          />
        </circle>
      </svg>
      <h2>Tu carrito está vacío</h2>
      <p>¡Agrega algunos productos para comenzar tu compra!</p>
    </div>
  );
};

export default EmptyCart;
