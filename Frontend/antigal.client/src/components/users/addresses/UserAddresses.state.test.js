import { cleanup, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { useOutletContext } from "react-router-dom";
import Swal from "sweetalert2";
import { beforeEach, describe, expect, test, vi } from "vitest";
import UserAddresses from "./UserAddresses";

vi.mock("react-router-dom", () => ({
  useOutletContext: vi.fn(),
}));

vi.mock("sweetalert2", () => ({
  default: {
    fire: vi.fn(),
  },
}));

const makeUser = (overrides = {}) => ({
  id: 1,
  name: "Lucas Martinez",
  direcciones: [
    { id: 1, address: "Calle Falsa 123" },
    { id: 2, address: "Avenida Libertador 456" },
  ],
  ...overrides,
});

let contextValue;

beforeEach(() => {
  cleanup();
  vi.clearAllMocks();
  contextValue = {
    user: makeUser(),
    setUserData: vi.fn(),
  };
  vi.mocked(useOutletContext).mockImplementation(() => contextValue);
});

describe("UserAddresses current-user contracts", () => {
  test("renders the current outlet user and follows a changed user on rerender", () => {
    const { rerender } = render(<UserAddresses />);

    expect(
      screen.getByRole("heading", { name: "Direcciones guardadas de Lucas Martinez" })
    ).toBeInTheDocument();
    expect(screen.getByText("Calle Falsa 123")).toBeInTheDocument();

    contextValue = {
      ...contextValue,
      user: makeUser({
        name: "Ana Torres",
        direcciones: [{ id: 3, address: "Diagonal 74 100" }],
      }),
    };

    rerender(<UserAddresses />);

    expect(
      screen.getByRole("heading", { name: "Direcciones guardadas de Ana Torres" })
    ).toBeInTheDocument();
    expect(screen.getByText("Diagonal 74 100")).toBeInTheDocument();
    expect(screen.queryByText("Calle Falsa 123")).not.toBeInTheDocument();
  });

  test("editing an address publishes the updated user through the outlet setter", () => {
    render(<UserAddresses />);

    fireEvent.click(screen.getAllByRole("button", { name: "Editar" })[0]);

    const input = screen.getByPlaceholderText("Nueva dirección");
    expect(input).toHaveValue("Calle Falsa 123");

    fireEvent.change(input, { target: { value: "Calle Nueva 999" } });
    fireEvent.click(screen.getByRole("button", { name: "Guardar cambios" }));

    expect(contextValue.setUserData).toHaveBeenCalledTimes(1);
    expect(contextValue.setUserData).toHaveBeenCalledWith({
      ...contextValue.user,
      direcciones: [
        { id: 1, address: "Calle Nueva 999" },
        { id: 2, address: "Avenida Libertador 456" },
      ],
    });
    expect(screen.queryByPlaceholderText("Nueva dirección")).not.toBeInTheDocument();
    expect(Swal.fire).toHaveBeenCalledWith(
      "¡Éxito!",
      "La dirección ha sido actualizada.",
      "success"
    );
  });

  test("confirmed deletion publishes the user without the deleted address", async () => {
    vi.mocked(Swal.fire).mockResolvedValueOnce({ isConfirmed: true });

    render(<UserAddresses />);

    fireEvent.click(screen.getAllByRole("button", { name: "Eliminar" })[0]);

    await waitFor(() => {
      expect(contextValue.setUserData).toHaveBeenCalledWith({
        ...contextValue.user,
        direcciones: [{ id: 2, address: "Avenida Libertador 456" }],
      });
    });

    expect(Swal.fire).toHaveBeenNthCalledWith(
      2,
      "Eliminado",
      "La dirección ha sido eliminada.",
      "success"
    );
  });
});
