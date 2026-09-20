import { cleanup, fireEvent, render, screen } from "@testing-library/react";
import { vi } from "vitest";
import CategoryForm from "./CategoryForm";

afterEach(() => {
  cleanup();
});

function renderForm({ category = null, onSave = vi.fn(), onClose = vi.fn() } = {}) {
  const key = category?.idCategoria ?? "new";
  return {
    onSave,
    onClose,
    ...render(
      <CategoryForm
        key={key}
        show
        onClose={onClose}
        onSave={onSave}
        category={category}
      />
    ),
  };
}

describe("CategoryForm state contracts", () => {
  test("initializes edit fields and existing image preview from the selected category", () => {
    renderForm({
      category: {
        idCategoria: 11,
        nombre: "Frutos secos",
        descripcion: "Selección premium",
        imagen: "data:image/png;base64,category-a",
      },
    });

    expect(screen.getByRole("heading", { name: "Editar Categoría" })).toBeInTheDocument();
    expect(screen.getByLabelText(/Nombre de la categoría/i)).toHaveValue("Frutos secos");
    expect(screen.getByLabelText(/Descripción/i)).toHaveValue("Selección premium");
    expect(screen.getByRole("img", { name: "Preview" })).toHaveAttribute(
      "src",
      "data:image/png;base64,category-a"
    );
  });

  test("a keyed category change reinitializes the form and new mode starts blank", () => {
    const onSave = vi.fn();
    const onClose = vi.fn();
    const first = {
      idCategoria: 12,
      nombre: "Cereales",
      descripcion: "Cereales integrales",
      imagen: "data:image/png;base64,first",
    };
    const second = {
      idCategoria: 13,
      nombre: "Semillas",
      descripcion: "Mix de semillas",
      imagen: "data:image/png;base64,second",
    };

    const { rerender } = render(
      <CategoryForm
        key={first.idCategoria}
        show
        onClose={onClose}
        onSave={onSave}
        category={first}
      />
    );

    fireEvent.change(screen.getByLabelText(/Nombre de la categoría/i), {
      target: { value: "Edición local" },
    });
    expect(screen.getByLabelText(/Nombre de la categoría/i)).toHaveValue("Edición local");

    rerender(
      <CategoryForm
        key={second.idCategoria}
        show
        onClose={onClose}
        onSave={onSave}
        category={second}
      />
    );

    expect(screen.getByLabelText(/Nombre de la categoría/i)).toHaveValue("Semillas");
    expect(screen.getByLabelText(/Descripción/i)).toHaveValue("Mix de semillas");
    expect(screen.getByRole("img", { name: "Preview" })).toHaveAttribute(
      "src",
      "data:image/png;base64,second"
    );

    rerender(
      <CategoryForm
        key="new"
        show
        onClose={onClose}
        onSave={onSave}
        category={null}
      />
    );

    expect(screen.getByRole("heading", { name: "Crear Nueva Categoría" })).toBeInTheDocument();
    expect(screen.getByLabelText(/Nombre de la categoría/i)).toHaveValue("");
    expect(screen.getByLabelText(/Descripción/i)).toHaveValue("");
    expect(screen.queryByRole("img", { name: "Preview" })).not.toBeInTheDocument();
  });

  test("submitting an edited category preserves the maintained FormData contract", () => {
    const onSave = vi.fn();
    const onClose = vi.fn();

    renderForm({
      category: {
        idCategoria: 21,
        nombre: "Avena",
        descripcion: "Descripción inicial",
        imagen: "",
      },
      onSave,
      onClose,
    });

    fireEvent.change(screen.getByLabelText(/Nombre de la categoría/i), {
      target: { value: "Avena premium" },
    });
    fireEvent.change(screen.getByLabelText(/Descripción/i), {
      target: { value: "Descripción actualizada" },
    });

    fireEvent.click(screen.getByRole("button", { name: "Actualizar" }));

    expect(onSave).toHaveBeenCalledTimes(1);
    const formData = onSave.mock.calls[0][0];
    expect(formData.get("idCategoria")).toBe("21");
    expect(JSON.parse(formData.get("category"))).toEqual({
      nombre: "Avena premium",
      descripcion: "Descripción actualizada",
    });
    expect(onClose).toHaveBeenCalledTimes(1);
  });
});
