import { fireEvent, render, screen } from "@testing-library/react";
import { vi } from "vitest";
import AdminProductItem from "./AdminProductItem";

const product = {
  idProducto: 11,
  nombre: "Avena",
  precio: 1250.5,
  imagenUrls: { $values: ["/images/oats.jpg"] },
  descripcion: "Avena integral",
  marca: "Antigal",
  stock: 8,
  disponible: true,
  codigoBarras: "779000011",
};

test("renders the API image without maintaining unused synchronized state", () => {
  render(<AdminProductItem product={product} onEdit={vi.fn()} onDelete={vi.fn()} />);

  expect(screen.getByRole("img", { name: "Avena" })).toHaveAttribute(
    "src",
    "/images/oats.jpg"
  );
  expect(screen.getByText("Precio: $1250.50")).toBeInTheDocument();
});

test("keeps the existing image-error fallback", () => {
  render(<AdminProductItem product={product} onEdit={vi.fn()} onDelete={vi.fn()} />);

  fireEvent.error(screen.getByRole("img", { name: "Avena" }));

  expect(screen.getByText("Sin Imagen")).toBeInTheDocument();
});

test("preserves admin edit and delete callbacks", () => {
  const onEdit = vi.fn();
  const onDelete = vi.fn();

  render(<AdminProductItem product={product} onEdit={onEdit} onDelete={onDelete} />);

  fireEvent.click(screen.getByRole("button", { name: "Modificar" }));
  fireEvent.click(screen.getByRole("button", { name: "Eliminar" }));

  expect(onEdit).toHaveBeenCalledWith(product);
  expect(onDelete).toHaveBeenCalledWith(11);
});
