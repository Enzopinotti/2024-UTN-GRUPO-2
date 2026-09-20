import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { beforeEach, describe, expect, test, vi } from "vitest";
import { CartContext } from "../../../contexts/CartContext";
import Product from "./Product";

const { addFavorite, removeFavorite } = vi.hoisted(() => ({
  addFavorite: vi.fn(),
  removeFavorite: vi.fn(),
}));

vi.mock("../../../contexts/FavoriteContext", () => ({
  useFavorites: () => ({
    favorites: [],
    addFavorite,
    removeFavorite,
  }),
}));

vi.mock("sweetalert2", () => ({
  default: {
    fire: vi.fn(),
  },
}));

const product = {
  id: 42,
  name: "avena premium",
  images: "/avena.png",
  categories: ["cereales"],
  price: 1200,
  salePrice: 1000,
  onSale: false,
};

beforeEach(() => {
  localStorage.clear();
  addFavorite.mockClear();
  removeFavorite.mockClear();
});

describe("Product favorite synchronization", () => {
  test("preserves initial non-favorite synchronization and toggles into favorites", async () => {
    render(
      <MemoryRouter>
        <CartContext.Provider value={{ addToCart: vi.fn() }}>
          <Product product={product} />
        </CartContext.Provider>
      </MemoryRouter>
    );

    await waitFor(() => {
      expect(removeFavorite).toHaveBeenCalledWith(product.id);
    });
    expect(localStorage.getItem(`liked-${product.id}`)).toBe("false");

    fireEvent.click(screen.getByRole("button"));

    await waitFor(() => {
      expect(addFavorite).toHaveBeenCalledWith(product);
    });
    expect(localStorage.getItem(`liked-${product.id}`)).toBe("true");
  });
});
