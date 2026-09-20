import { cleanup, fireEvent, render, screen } from "@testing-library/react";
import { vi } from "vitest";
import CategoryItem from "./categories/CategoryItem";
import AdminProductItem from "./products/AdminProductItem";

afterEach(() => {
  cleanup();
});

describe("admin image item contracts", () => {
  test("CategoryItem follows the current category image and preserves actions", () => {
    const onEdit = vi.fn();
    const onDelete = vi.fn();
    const category = {
      idCategoria: 7,
      nombre: "Frutos secos",
      descripcion: "Selección premium",
      imagenUrl: "https://cdn.example.com/category-a.jpg",
    };

    const { rerender } = render(
      <CategoryItem category={category} onEdit={onEdit} onDelete={onDelete} />
    );

    expect(screen.getByRole("img", { name: "Frutos secos" })).toHaveAttribute(
      "src",
      "https://cdn.example.com/category-a.jpg"
    );

    const nextCategory = {
      ...category,
      imagenUrl: "https://cdn.example.com/category-b.jpg",
    };

    rerender(
      <CategoryItem
        category={nextCategory}
        onEdit={onEdit}
        onDelete={onDelete}
      />
    );

    expect(screen.getByRole("img", { name: "Frutos secos" })).toHaveAttribute(
      "src",
      "https://cdn.example.com/category-b.jpg"
    );

    fireEvent.click(screen.getByRole("button", { name: "Modificar" }));
    fireEvent.click(screen.getByRole("button", { name: "Eliminar" }));

    expect(onEdit).toHaveBeenCalledWith(nextCategory);
    expect(onDelete).toHaveBeenCalledWith(7);
  });

  test("CategoryItem falls back when its image cannot be loaded", () => {
    render(
      <CategoryItem
        category={{
          idCategoria: 8,
          nombre: "Cereales",
          descripcion: "Cereales",
          imagenUrl: "https://cdn.example.com/missing-category.jpg",
        }}
        onEdit={vi.fn()}
        onDelete={vi.fn()}
      />
    );

    fireEvent.error(screen.getByRole("img", { name: "Cereales" }));

    expect(screen.getByText("Sin Imagen")).toBeInTheDocument();
  });

  test("AdminProductItem renders the first backend image and preserves actions", () => {
    const onEdit = vi.fn();
    const onDelete = vi.fn();
    const product = {
      idProducto: 42,
      nombre: "Granola",
      precio: 3500,
      imagenUrls: {
        $values: [
          "https://cdn.example.com/granola-a.jpg",
          "https://cdn.example.com/granola-b.jpg",
        ],
      },
      descripcion: "Granola artesanal",
      marca: "Antigal",
      stock: 12,
      disponible: true,
      codigoBarras: "7790000000420",
    };

    render(
      <AdminProductItem product={product} onEdit={onEdit} onDelete={onDelete} />
    );

    expect(screen.getByRole("img", { name: "Granola" })).toHaveAttribute(
      "src",
      "https://cdn.example.com/granola-a.jpg"
    );

    fireEvent.click(screen.getByRole("button", { name: "Modificar" }));
    fireEvent.click(screen.getByRole("button", { name: "Eliminar" }));

    expect(onEdit).toHaveBeenCalledWith(product);
    expect(onDelete).toHaveBeenCalledWith(42);
  });

  test("AdminProductItem falls back when its rendered image cannot be loaded", () => {
    render(
      <AdminProductItem
        product={{
          idProducto: 43,
          nombre: "Avena",
          precio: 2100,
          imagenUrls: {
            $values: ["https://cdn.example.com/missing-product.jpg"],
          },
          descripcion: "Avena",
          marca: "Antigal",
          stock: 5,
          disponible: true,
          codigoBarras: "7790000000437",
        }}
        onEdit={vi.fn()}
        onDelete={vi.fn()}
      />
    );

    fireEvent.error(screen.getByRole("img", { name: "Avena" }));

    expect(screen.getByText("Sin Imagen")).toBeInTheDocument();
  });
});
