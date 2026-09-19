import { cleanup, render, screen } from "@testing-library/react";
import { vi } from "vitest";
import App from "./App";

vi.mock("./components/layout/Header", () => ({
  default: () => <header data-testid="app-header" />,
}));
vi.mock("./components/layout/Main", () => ({
  default: ({ children }) => <main data-testid="app-main">{children}</main>,
}));
vi.mock("./components/layout/Footer", () => ({
  default: () => <footer data-testid="app-footer" />,
}));

vi.mock("./contexts/CartContext", () => ({
  CartProvider: ({ children }) => children,
}));
vi.mock("./contexts/FavoriteContext", () => ({
  FavoriteProvider: ({ children }) => children,
}));
vi.mock("react-toastify", () => ({
  ToastContainer: () => null,
}));

vi.mock("./pages/Home", () => ({
  default: () => <div data-testid="route-home" />,
}));
vi.mock("./components/products/productList/ProductListContainer", () => ({
  default: () => <div data-testid="route-products" />,
}));
vi.mock("./components/products/productDetail/ProductDetailContainer", () => ({
  default: () => <div data-testid="route-product-detail" />,
}));
vi.mock("./pages/CartPage", () => ({
  default: () => <div data-testid="route-cart" />,
}));

vi.mock("./components/users/UserLayout", async () => {
  const { Outlet } = await vi.importActual("react-router-dom");
  return {
    default: () => (
      <section data-testid="route-profile-layout">
        <Outlet />
      </section>
    ),
  };
});
vi.mock("./pages/profile/Profile", () => ({
  default: () => <div data-testid="route-profile" />,
}));
vi.mock("./pages/profile/Orders", () => ({
  default: () => <div data-testid="route-profile-orders" />,
}));
vi.mock("./pages/profile/Favorites", () => ({
  default: () => <div data-testid="route-profile-favorites" />,
}));
vi.mock("./components/users/addresses/UserAddresses", () => ({
  default: () => <div data-testid="route-profile-addresses" />,
}));

vi.mock("./components/admin/dashboard/AdminDashboard", async () => {
  const { Outlet } = await vi.importActual("react-router-dom");
  return {
    default: () => (
      <section data-testid="route-admin-layout">
        <Outlet />
      </section>
    ),
  };
});
vi.mock("./components/admin/categories/CategoryListContainer", () => ({
  default: () => <div data-testid="route-admin-categories" />,
}));
vi.mock("./components/admin/products/ProductListContainer", () => ({
  default: () => <div data-testid="route-admin-products" />,
}));
vi.mock("./components/admin/users/AdminUserListContainer", () => ({
  default: () => <div data-testid="route-admin-users" />,
}));
vi.mock("./components/admin/messages/MessageListContainer", () => ({
  default: () => <div data-testid="route-admin-messages" />,
}));

vi.mock("./pages/auth/Login", () => ({
  default: () => <div data-testid="route-login" />,
}));
vi.mock("./pages/auth/ResetearContrasenia", () => ({
  default: () => <div data-testid="route-reset-password" />,
}));
vi.mock("./pages/auth/RecuperarContrasenia", () => ({
  default: () => <div data-testid="route-forgot-password" />,
}));
vi.mock("./pages/auth/Registro", () => ({
  default: () => <div data-testid="route-register" />,
}));
vi.mock("./pages/auth/Logout", () => ({
  default: () => <div data-testid="route-logout" />,
}));

vi.mock("./pages/SobreNosotros", () => ({
  default: () => <div data-testid="route-about" />,
}));
vi.mock("./pages/TiendaFisica", () => ({
  default: () => <div data-testid="route-store" />,
}));
vi.mock("./pages/Contact", () => ({
  default: () => <div data-testid="route-contact" />,
}));
vi.mock("./pages/PrivacyPolicy", () => ({
  default: () => <div data-testid="route-privacy" />,
}));
vi.mock("./components/common/ConfirmEmail", () => ({
  default: () => <div data-testid="route-confirm-email" />,
}));
vi.mock("./components/common/RegistrationSuccess", () => ({
  default: () => <div data-testid="route-registration-success" />,
}));
vi.mock("./pages/CheckoutPage", () => ({
  default: () => <div data-testid="route-checkout" />,
}));
vi.mock("./components/NotFound", () => ({
  default: () => <div data-testid="route-not-found" />,
}));

function renderAt(path) {
  window.history.pushState({}, "", path);
  return render(<App />);
}

afterEach(() => {
  cleanup();
  window.history.pushState({}, "", "/");
});

test("renders the shared application shell and home route", () => {
  renderAt("/");

  expect(screen.getByTestId("app-header")).toBeInTheDocument();
  expect(screen.getByTestId("app-main")).toBeInTheDocument();
  expect(screen.getByTestId("app-footer")).toBeInTheDocument();
  expect(screen.getByTestId("route-home")).toBeInTheDocument();
});

test.each([
  ["/products", "route-products"],
  ["/products/abc-123", "route-product-detail"],
  ["/cart", "route-cart"],
  ["/login", "route-login"],
  ["/resetearContrasenia", "route-reset-password"],
  ["/recuperarContrasenia", "route-forgot-password"],
  ["/register", "route-register"],
  ["/logout", "route-logout"],
  ["/sobre-nosotros", "route-about"],
  ["/tienda-fisica", "route-store"],
  ["/contacto", "route-contact"],
  ["/politica-de-privacidad", "route-privacy"],
  ["/authentication/confirm-email", "route-confirm-email"],
  ["/registration-success", "route-registration-success"],
  ["/checkout", "route-checkout"],
])("maps public path %s to its maintained route", (path, testId) => {
  renderAt(path);
  expect(screen.getByTestId(testId)).toBeInTheDocument();
});

test.each([
  ["/profile", "route-profile"],
  ["/profile/orders", "route-profile-orders"],
  ["/profile/favorites", "route-profile-favorites"],
  ["/profile/addresses", "route-profile-addresses"],
])("preserves nested profile route %s", (path, testId) => {
  renderAt(path);

  expect(screen.getByTestId("route-profile-layout")).toBeInTheDocument();
  expect(screen.getByTestId(testId)).toBeInTheDocument();
});

test.each([
  ["/admin/categories", "route-admin-categories"],
  ["/admin/products", "route-admin-products"],
  ["/admin/users", "route-admin-users"],
  ["/admin/messages", "route-admin-messages"],
])("preserves nested admin route %s", (path, testId) => {
  renderAt(path);

  expect(screen.getByTestId("route-admin-layout")).toBeInTheDocument();
  expect(screen.getByTestId(testId)).toBeInTheDocument();
});

test("maps unknown paths to the maintained not-found route", () => {
  renderAt("/route-that-does-not-exist");
  expect(screen.getByTestId("route-not-found")).toBeInTheDocument();
});
