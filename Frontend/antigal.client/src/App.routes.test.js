import React from "react";
import { cleanup, render, screen } from "@testing-library/react";
import App from "./App";

jest.mock("./components/layout/Header", () => () =>
  require("react").createElement("header", { "data-testid": "app-header" })
);
jest.mock("./components/layout/Main", () => ({ children }) =>
  require("react").createElement("main", { "data-testid": "app-main" }, children)
);
jest.mock("./components/layout/Footer", () => () =>
  require("react").createElement("footer", { "data-testid": "app-footer" })
);

jest.mock("./contexts/CartContext", () => ({
  CartProvider: ({ children }) =>
    require("react").createElement(require("react").Fragment, null, children),
}));
jest.mock("./contexts/FavoriteContext", () => ({
  FavoriteProvider: ({ children }) =>
    require("react").createElement(require("react").Fragment, null, children),
}));
jest.mock("react-toastify", () => ({
  ToastContainer: () => null,
}));

jest.mock("./pages/Home", () => () =>
  require("react").createElement("div", { "data-testid": "route-home" })
);
jest.mock("./components/products/productList/ProductListContainer", () => () =>
  require("react").createElement("div", { "data-testid": "route-products" })
);
jest.mock("./components/products/productDetail/ProductDetailContainer", () => () =>
  require("react").createElement("div", { "data-testid": "route-product-detail" })
);
jest.mock("./pages/CartPage", () => () =>
  require("react").createElement("div", { "data-testid": "route-cart" })
);

jest.mock("./components/users/UserLayout", () => () => {
  const ReactRuntime = require("react");
  const { Outlet } = require("react-router-dom");
  return ReactRuntime.createElement(
    "section",
    { "data-testid": "route-profile-layout" },
    ReactRuntime.createElement(Outlet)
  );
});
jest.mock("./pages/profile/Profile", () => () =>
  require("react").createElement("div", { "data-testid": "route-profile" })
);
jest.mock("./pages/profile/Orders", () => () =>
  require("react").createElement("div", { "data-testid": "route-profile-orders" })
);
jest.mock("./pages/profile/Favorites", () => () =>
  require("react").createElement("div", { "data-testid": "route-profile-favorites" })
);
jest.mock("./components/users/addresses/UserAddresses", () => () =>
  require("react").createElement("div", { "data-testid": "route-profile-addresses" })
);

jest.mock("./components/admin/dashboard/AdminDashboard", () => () => {
  const ReactRuntime = require("react");
  const { Outlet } = require("react-router-dom");
  return ReactRuntime.createElement(
    "section",
    { "data-testid": "route-admin-layout" },
    ReactRuntime.createElement(Outlet)
  );
});
jest.mock("./components/admin/categories/CategoryListContainer", () => () =>
  require("react").createElement("div", { "data-testid": "route-admin-categories" })
);
jest.mock("./components/admin/products/ProductListContainer", () => () =>
  require("react").createElement("div", { "data-testid": "route-admin-products" })
);
jest.mock("./components/admin/users/AdminUserListContainer", () => () =>
  require("react").createElement("div", { "data-testid": "route-admin-users" })
);
jest.mock("./components/admin/messages/MessageListContainer", () => () =>
  require("react").createElement("div", { "data-testid": "route-admin-messages" })
);

jest.mock("./pages/auth/Login", () => () =>
  require("react").createElement("div", { "data-testid": "route-login" })
);
jest.mock("./pages/auth/ResetearContrasenia", () => () =>
  require("react").createElement("div", { "data-testid": "route-reset-password" })
);
jest.mock("./pages/auth/RecuperarContrasenia", () => () =>
  require("react").createElement("div", { "data-testid": "route-forgot-password" })
);
jest.mock("./pages/auth/Registro", () => () =>
  require("react").createElement("div", { "data-testid": "route-register" })
);
jest.mock("./pages/auth/Logout", () => () =>
  require("react").createElement("div", { "data-testid": "route-logout" })
);

jest.mock("./pages/SobreNosotros", () => () =>
  require("react").createElement("div", { "data-testid": "route-about" })
);
jest.mock("./pages/TiendaFisica", () => () =>
  require("react").createElement("div", { "data-testid": "route-store" })
);
jest.mock("./pages/Contact", () => () =>
  require("react").createElement("div", { "data-testid": "route-contact" })
);
jest.mock("./pages/PrivacyPolicy", () => () =>
  require("react").createElement("div", { "data-testid": "route-privacy" })
);
jest.mock("./components/common/ConfirmEmail", () => () =>
  require("react").createElement("div", { "data-testid": "route-confirm-email" })
);
jest.mock("./components/common/RegistrationSuccess", () => () =>
  require("react").createElement("div", { "data-testid": "route-registration-success" })
);
jest.mock("./pages/CheckoutPage", () => () =>
  require("react").createElement("div", { "data-testid": "route-checkout" })
);
jest.mock("./components/NotFound", () => () =>
  require("react").createElement("div", { "data-testid": "route-not-found" })
);

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
