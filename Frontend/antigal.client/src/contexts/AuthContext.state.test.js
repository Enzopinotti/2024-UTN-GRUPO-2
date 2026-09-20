import { cleanup, fireEvent, render, screen } from "@testing-library/react";
import { useContext } from "react";
import { jwtDecode } from "jwt-decode";
import { beforeEach, describe, expect, test, vi } from "vitest";
import { AuthContext, AuthProvider } from "./AuthContext";

vi.mock("jwt-decode", () => ({
  jwtDecode: vi.fn(),
}));

const futureExp = 4102444800;

const Probe = () => {
  const { auth, login, logout } = useContext(AuthContext);

  return (
    <>
      <output data-testid="auth-state">{JSON.stringify(auth)}</output>
      <button
        type="button"
        onClick={() => login("login-token", "refresh-token")}
      >
        Login
      </button>
      <button type="button" onClick={logout}>
        Logout
      </button>
    </>
  );
};

const renderAuth = () =>
  render(
    <AuthProvider>
      <Probe />
    </AuthProvider>
  );

const readAuth = () => JSON.parse(screen.getByTestId("auth-state").textContent);

beforeEach(() => {
  cleanup();
  localStorage.clear();
  vi.clearAllMocks();

  vi.mocked(jwtDecode).mockImplementation((token) => {
    if (token === "valid-token") {
      return { sub: "valid-user", exp: futureExp };
    }
    if (token === "expired-token") {
      return { sub: "expired-user", exp: 1 };
    }
    if (token === "login-token") {
      return { sub: "login-user", exp: futureExp };
    }
    throw new Error("invalid token");
  });
});

describe("AuthProvider persistence contracts", () => {
  test("starts unauthenticated when no persisted access token exists", () => {
    renderAuth();

    expect(readAuth()).toEqual({
      accessToken: null,
      user: null,
    });
    expect(jwtDecode).not.toHaveBeenCalled();
  });

  test("hydrates a valid persisted access token", () => {
    localStorage.setItem("accessToken", "valid-token");

    renderAuth();

    expect(jwtDecode).toHaveBeenCalledWith("valid-token");
    expect(readAuth()).toEqual({
      accessToken: "valid-token",
      user: { sub: "valid-user", exp: futureExp },
    });
    expect(localStorage.getItem("accessToken")).toBe("valid-token");
  });

  test("clears expired persisted credentials and remains unauthenticated", () => {
    localStorage.setItem("accessToken", "expired-token");
    localStorage.setItem("refreshToken", "stale-refresh");

    renderAuth();

    expect(readAuth()).toEqual({
      accessToken: null,
      user: null,
    });
    expect(localStorage.getItem("accessToken")).toBeNull();
    expect(localStorage.getItem("refreshToken")).toBeNull();
  });

  test("clears malformed persisted credentials and remains unauthenticated", () => {
    localStorage.setItem("accessToken", "broken-token");
    localStorage.setItem("refreshToken", "stale-refresh");

    renderAuth();

    expect(readAuth()).toEqual({
      accessToken: null,
      user: null,
    });
    expect(localStorage.getItem("accessToken")).toBeNull();
    expect(localStorage.getItem("refreshToken")).toBeNull();
  });

  test("login publishes decoded auth and persists both tokens", () => {
    renderAuth();

    fireEvent.click(screen.getByRole("button", { name: "Login" }));

    expect(jwtDecode).toHaveBeenCalledWith("login-token");
    expect(readAuth()).toEqual({
      accessToken: "login-token",
      refreshToken: "refresh-token",
      user: { sub: "login-user", exp: futureExp },
    });
    expect(localStorage.getItem("accessToken")).toBe("login-token");
    expect(localStorage.getItem("refreshToken")).toBe("refresh-token");
  });

  test("logout clears auth state and persisted credentials", () => {
    renderAuth();
    fireEvent.click(screen.getByRole("button", { name: "Login" }));

    fireEvent.click(screen.getByRole("button", { name: "Logout" }));

    expect(readAuth()).toEqual({
      accessToken: null,
      user: null,
    });
    expect(localStorage.getItem("accessToken")).toBeNull();
    expect(localStorage.getItem("refreshToken")).toBeNull();
  });
});
