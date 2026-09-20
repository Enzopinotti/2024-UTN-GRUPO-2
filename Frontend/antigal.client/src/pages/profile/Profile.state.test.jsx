import { cleanup, fireEvent, render, screen } from "@testing-library/react";
import { useOutletContext } from "react-router-dom";
import { beforeEach, describe, expect, test, vi } from "vitest";
import Profile from "./Profile";

vi.mock("react-router-dom", () => ({
  useOutletContext: vi.fn(),
}));

vi.mock("sweetalert2", () => ({
  default: {
    fire: vi.fn(),
  },
}));

vi.mock("../../components/users/ProfilePictureModal", () => ({
  default: ({ onUploadComplete }) => (
    <button
      type="button"
      onClick={() => onUploadComplete("https://cdn.example.com/new-profile.jpg")}
    >
      Completar foto
    </button>
  ),
}));

const makeUser = (overrides = {}) => ({
  id: 1,
  user: "usuario1",
  name: "Lucas Martinez",
  picture: "https://cdn.example.com/profile-a.jpg",
  email: "lucas@example.com",
  fechaNacimiento: "1999-01-01",
  telefono: "+1234567890",
  genero: "Masculino",
  dni: "41167000",
  direcciones: [],
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

describe("Profile outlet-user contracts", () => {
  test("renders the current outlet user and follows a changed user while not editing", () => {
    const { rerender } = render(<Profile />);

    expect(screen.getByRole("heading", { name: "Lucas Martinez" })).toBeInTheDocument();

    contextValue = {
      ...contextValue,
      user: makeUser({
        id: 2,
        user: "ana",
        name: "Ana Torres",
        email: "ana@example.com",
      }),
    };

    rerender(<Profile />);

    expect(screen.getByRole("heading", { name: "Ana Torres" })).toBeInTheDocument();
    expect(screen.getAllByText("ana").length).toBeGreaterThan(0);
  });

  test("keeps profile edits local until Guardar publishes through the real outlet setter", () => {
    render(<Profile />);

    fireEvent.click(screen.getByRole("button", { name: "Editar Perfil" }));

    const nameInput = screen.getByDisplayValue("Lucas Martinez");
    fireEvent.change(nameInput, { target: { value: "Lucas Editado" } });

    const birthDateInput = screen.getByDisplayValue("1999-01-01");
    fireEvent.change(birthDateInput, { target: { value: "2000-02-03" } });

    expect(contextValue.setUserData).not.toHaveBeenCalled();

    fireEvent.click(screen.getByRole("button", { name: "Guardar" }));

    expect(contextValue.setUserData).toHaveBeenCalledTimes(1);
    expect(contextValue.setUserData).toHaveBeenCalledWith({
      ...contextValue.user,
      name: "Lucas Editado",
      fechaNacimiento: "2000-02-03",
    });
    expect(screen.getByRole("button", { name: "Editar Perfil" })).toBeInTheDocument();
  });

  test("publishes profile-picture completion through the real outlet setter", () => {
    render(<Profile />);

    fireEvent.click(screen.getByRole("button", { name: "Completar foto" }));

    expect(contextValue.setUserData).toHaveBeenCalledTimes(1);
    const updater = contextValue.setUserData.mock.calls[0][0];
    expect(typeof updater).toBe("function");
    expect(updater(contextValue.user)).toEqual({
      ...contextValue.user,
      picture: "https://cdn.example.com/new-profile.jpg",
    });
  });
});
