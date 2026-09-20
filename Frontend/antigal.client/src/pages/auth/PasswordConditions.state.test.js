import { cleanup, fireEvent, render, screen } from "@testing-library/react";
import { MemoryRouter, Route, Routes } from "react-router-dom";
import { afterEach, describe, expect, test } from "vitest";
import Registro from "./Registro";
import ResetearContrasenia from "./ResetearContrasenia";

afterEach(() => {
  cleanup();
});

const assertWeakPasswordState = () => {
  expect(screen.getByText(/Al menos 8 caracteres/)).toHaveClass("condition", "unmet");
  expect(screen.getByText(/Al menos una letra minúscula/)).toHaveClass("condition", "met");
  expect(screen.getByText(/Al menos una letra mayúscula/)).toHaveClass("condition", "unmet");
  expect(screen.getByText(/Al menos un número/)).toHaveClass("condition", "unmet");
  expect(screen.getByText(/Al menos un carácter especial/)).toHaveClass("condition", "unmet");
  expect(screen.queryByText("¡La contraseña es segura!")).not.toBeInTheDocument();
};

const assertStrongPasswordState = () => {
  expect(screen.queryByText(/Al menos 8 caracteres/)).not.toBeInTheDocument();
  expect(screen.getByText("¡La contraseña es segura!")).toBeInTheDocument();
};

describe("password condition reactivity contracts", () => {
  test("Registro reacts from weak to strong passwords", () => {
    render(
      <MemoryRouter>
        <Registro />
      </MemoryRouter>
    );

    const password = screen.getByLabelText("Contraseña:");

    fireEvent.change(password, { target: { value: "abc" } });
    assertWeakPasswordState();

    fireEvent.change(password, { target: { value: "Abcd123!" } });
    assertStrongPasswordState();
  });

  test("ResetearContrasenia reacts from weak to strong passwords", () => {
    render(
      <MemoryRouter initialEntries={["/reset/token-123"]}>
        <Routes>
          <Route path="/reset/:token" element={<ResetearContrasenia />} />
        </Routes>
      </MemoryRouter>
    );

    const password = screen.getByLabelText("Nueva Contraseña:");

    fireEvent.change(password, { target: { value: "abc" } });
    assertWeakPasswordState();

    fireEvent.change(password, { target: { value: "Abcd123!" } });
    assertStrongPasswordState();
  });
});
