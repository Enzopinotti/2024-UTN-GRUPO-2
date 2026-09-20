// src/App.js
import { lazy, Suspense } from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import Header from "./components/layout/Header";
import Main from "./components/layout/Main";
import Footer from "./components/layout/Footer";
import { CartProvider } from "./contexts/CartContext";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { FavoriteProvider } from "./contexts/FavoriteContext";

const Home = lazy(() => import("./pages/Home"));
const ProductListContainer = lazy(() =>
  import("./components/products/productList/ProductListContainer")
);
const ProductDetailContainer = lazy(() =>
  import("./components/products/productDetail/ProductDetailContainer")
);
const CartPage = lazy(() => import("./pages/CartPage"));

const UserLayout = lazy(() => import("./components/users/UserLayout"));
const Profile = lazy(() => import("./pages/profile/Profile"));
const Orders = lazy(() => import("./pages/profile/Orders"));
const Favorites = lazy(() => import("./pages/profile/Favorites"));
const UserAddresses = lazy(() =>
  import("./components/users/addresses/UserAddresses")
);

const AdminDashboard = lazy(() =>
  import("./components/admin/dashboard/AdminDashboard")
);
const CategoryListContainer = lazy(() =>
  import("./components/admin/categories/CategoryListContainer")
);
const AdminProductListContainer = lazy(() =>
  import("./components/admin/products/ProductListContainer")
);
const AdminUserListContainer = lazy(() =>
  import("./components/admin/users/AdminUserListContainer")
);
const MessageListContainer = lazy(() =>
  import("./components/admin/messages/MessageListContainer")
);

const Login = lazy(() => import("./pages/auth/Login"));
const ResetearContrasenia = lazy(() =>
  import("./pages/auth/ResetearContrasenia")
);
const RecuperarContrasenia = lazy(() =>
  import("./pages/auth/RecuperarContrasenia")
);
const Registro = lazy(() => import("./pages/auth/Registro"));
const Logout = lazy(() => import("./pages/auth/Logout"));
const SobreNosotros = lazy(() => import("./pages/SobreNosotros"));
const TiendaFisica = lazy(() => import("./pages/TiendaFisica"));
const Contact = lazy(() => import("./pages/Contact"));
const PrivacyPolicy = lazy(() => import("./pages/PrivacyPolicy"));
const ConfirmEmail = lazy(() => import("./components/common/ConfirmEmail"));
const RegistrationSuccess = lazy(() =>
  import("./components/common/RegistrationSuccess")
);
const CheckoutPage = lazy(() => import("./pages/CheckoutPage"));
const NotFound = lazy(() => import("./components/NotFound"));

function RouteFallback() {
  return (
    <div role="status" aria-live="polite" data-testid="route-loading">
      Cargando...
    </div>
  );
}

function App() {
  return (
    <CartProvider>
      <FavoriteProvider>
        <Router>
          <Header />
          <Main>
            <Suspense fallback={<RouteFallback />}>
              <Routes>
                <Route path="/" element={<Home />} />
                <Route path="/products" element={<ProductListContainer />} />
                <Route
                  path="/products/:id"
                  element={<ProductDetailContainer />}
                />
                <Route path="/cart" element={<CartPage />} />

                {/* Rutas de usuario */}
                <Route path="/profile" element={<UserLayout />}>
                  <Route index element={<Profile />} />
                  <Route path="orders" element={<Orders />} />
                  <Route path="favorites" element={<Favorites />} />
                  <Route path="addresses" element={<UserAddresses />} />
                </Route>

                {/* Ruta protegida para el admin */}
                <Route path="/admin/*" element={<AdminDashboard />}>
                  {/* Subrutas del dashboard */}
                  <Route
                    path="categories"
                    element={<CategoryListContainer />}
                  />
                  <Route
                    path="products"
                    element={<AdminProductListContainer />}
                  />
                  <Route
                    path="users"
                    element={<AdminUserListContainer />}
                  />
                  <Route
                    path="messages"
                    element={<MessageListContainer />}
                  />
                </Route>

                {/* Rutas de autenticación */}
                <Route path="/login" element={<Login />} />
                <Route
                  path="/resetearContrasenia"
                  element={<ResetearContrasenia />}
                />
                <Route
                  path="/recuperarContrasenia"
                  element={<RecuperarContrasenia />}
                />
                <Route path="/register" element={<Registro />} />
                <Route path="/logout" element={<Logout />} />
                <Route path="/sobre-nosotros" element={<SobreNosotros />} />
                <Route path="/tienda-fisica" element={<TiendaFisica />} />
                <Route path="/contacto" element={<Contact />} />
                <Route
                  path="/politica-de-privacidad"
                  element={<PrivacyPolicy />}
                />
                <Route
                  path="/authentication/confirm-email"
                  element={<ConfirmEmail />}
                />
                <Route
                  path="/registration-success"
                  element={<RegistrationSuccess />}
                />
                <Route path="/checkout" element={<CheckoutPage />} />
                <Route path="*" element={<NotFound />} />
              </Routes>
            </Suspense>
          </Main>
          <Footer />
        </Router>

        <ToastContainer />
      </FavoriteProvider>
    </CartProvider>
  );
}

export default App;
