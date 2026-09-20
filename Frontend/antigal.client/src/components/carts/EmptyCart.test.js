import { render, screen } from "@testing-library/react";
import EmptyCart from "./EmptyCart";

describe("EmptyCart", () => {
  test("keeps the empty-cart message and native animated illustration", () => {
    render(<EmptyCart />);

    expect(screen.getByRole("img", { name: "Carrito vacío" })).toBeInTheDocument();
    expect(screen.getByTestId("empty-cart-animation").tagName).toBe("svg");
    expect(screen.getByRole("heading", { name: "Tu carrito está vacío" })).toBeInTheDocument();
    expect(
      screen.getByText("¡Agrega algunos productos para comenzar tu compra!")
    ).toBeInTheDocument();
  });
});
