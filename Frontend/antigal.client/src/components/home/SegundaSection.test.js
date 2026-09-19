import { render, screen } from "@testing-library/react";
import { afterEach, beforeEach, vi } from "vitest";
import SegundaSection from "./SegundaSection";

vi.mock("react-swipeable", () => ({
  useSwipeable: () => ({}),
}));

vi.mock("../common/OfferCard", () => ({
  default: ({ producto }) => (
    <div data-testid={`offer-${producto.idProducto}`}>{producto.nombre}</div>
  ),
}));

beforeEach(() => {
  Object.defineProperty(window, "innerWidth", {
    configurable: true,
    value: 1200,
  });
  vi.spyOn(console, "log").mockImplementation(() => {});
});

afterEach(() => {
  vi.restoreAllMocks();
});

test("loads recommended products and leaves the carousel at its initial page", async () => {
  global.fetch = vi.fn().mockResolvedValue({
    ok: true,
    json: async () => ({
      data: {
        $values: [
          { idProducto: 1, nombre: "Avena" },
          { idProducto: 2, nombre: "Granola" },
        ],
      },
    }),
  });

  render(<SegundaSection />);

  expect(screen.getByText("Cargando productos...")).toBeInTheDocument();
  expect(await screen.findByText("TOP PRODUCTOS RECOMENDADOS")).toBeInTheDocument();
  expect(screen.getAllByText("Avena").length).toBeGreaterThan(0);
  expect(screen.getAllByText("Granola").length).toBeGreaterThan(0);
  expect(global.fetch).toHaveBeenCalledWith(
    "https://www.antigal.somee.com/api/Product/home"
  );
});

test("preserves the API failure state", async () => {
  global.fetch = vi.fn().mockResolvedValue({
    ok: false,
  });

  render(<SegundaSection />);

  expect(
    await screen.findByText("Error: Error al obtener productos")
  ).toBeInTheDocument();
});
