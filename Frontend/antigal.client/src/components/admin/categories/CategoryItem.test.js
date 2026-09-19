import { fireEvent, render, screen } from "@testing-library/react";
import { vi } from "vitest";
import CategoryItem from "./CategoryItem";

const category = {
  idCategoria: 7,
  nombre: "Frutos secos",
  descripcion: "Selección premium",
  imagenUrl: "/images/category.jpg",
};

test("renders the category image directly from the maintained prop", () => {
  render(<CategoryItem category={category} onEdit={vi.fn()} onDelete={vi.fn()} />);

  expect(screen.getByRole("img", { name: "Frutos secos" })).toHaveAttribute(
    "src",
    "/images/category.jpg"
  );
});

test("keeps the existing image-error fallback", () => {
  render(<CategoryItem category={category} onEdit={vi.fn()} onDelete={vi.fn()} />);

  fireEvent.error(screen.getByRole("img", { name: "Frutos secos" }));

  expect(screen.getByText("Sin Imagen")).toBeInTheDocument();
});

test("preserves edit and delete callbacks", () => {
  const onEdit = vi.fn();
  const onDelete = vi.fn();

  render(<CategoryItem category={category} onEdit={onEdit} onDelete={onDelete} />);

  fireEvent.click(screen.getByRole("button", { name: "Modificar" }));
  fireEvent.click(screen.getByRole("button", { name: "Eliminar" }));

  expect(onEdit).toHaveBeenCalledWith(category);
  expect(onDelete).toHaveBeenCalledWith(7);
});
