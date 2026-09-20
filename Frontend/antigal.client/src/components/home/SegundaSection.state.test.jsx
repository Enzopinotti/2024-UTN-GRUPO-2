import { act, render, screen, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, test, vi } from "vitest";
import SegundaSection from "./SegundaSection";

vi.mock("../common/OfferCard", () => ({
  default: ({ producto }) => (
    <div data-testid="offer-card">{producto.nombre}</div>
  ),
}));

vi.mock("react-swipeable", () => ({
  useSwipeable: () => ({}),
}));

const productos = [
  { idProducto: 1, nombre: "Producto Uno" },
  { idProducto: 2, nombre: "Producto Dos" },
  { idProducto: 3, nombre: "Producto Tres" },
  { idProducto: 4, nombre: "Producto Cuatro" },
];

beforeEach(() => {
  Object.defineProperty(window, "innerWidth", {
    configurable: true,
    writable: true,
    value: 500,
  });

  globalThis.fetch = vi.fn(() =>
    Promise.resolve({
      ok: true,
      json: () =>
        Promise.resolve({
          data: { $values: productos },
        }),
    })
  );
});

describe("SegundaSection carousel state", () => {
  test("starts from the first product after loading and preserves responsive visibility", async () => {
    render(<SegundaSection />);

    expect(screen.getByText("Cargando productos...")).toBeInTheDocument();

    await waitFor(() => {
      expect(screen.getAllByTestId("offer-card")).toHaveLength(1);
    });
    expect(screen.getByText("Producto Uno")).toBeInTheDocument();

    act(() => {
      window.innerWidth = 1200;
      window.dispatchEvent(new Event("resize"));
    });

    await waitFor(() => {
      expect(screen.getAllByTestId("offer-card")).toHaveLength(3);
    });

    expect(screen.getByText("Producto Uno")).toBeInTheDocument();
    expect(screen.getByText("Producto Dos")).toBeInTheDocument();
    expect(screen.getByText("Producto Tres")).toBeInTheDocument();
  });
});
