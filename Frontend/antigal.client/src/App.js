// src/App.js
<<<<<<< HEAD
import React from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import Header from './components/layout/Header';
import Main from './components/layout/Main';
import Footer from './components/layout/Footer';
import Home from './pages/Home';
import ProductListContainer from './components/products/productList/ProductListContainer';
import ProductDetailContainer from './components/products/productDetail/ProductDetailContainer';
import AdminDashboard from './components/admin/dashboard/AdminDashboard';
import CategoryListContainer from './components/admin/categories/CategoryListContainer';
import AdminProductListContainer from './components/admin/products/ProductListContainer';
import { CartProvider } from './contexts/CartContext';
import { ToastContainer, toast } from "react-toastify"; // Importación de toast y ToastContainer
import "react-toastify/dist/ReactToastify.css"; // Estilos de react-toastify


=======
import React from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import Header from "./components/layout/Header";
import Main from "./components/layout/Main";
import Footer from "./components/layout/Footer";
import Home from "./pages/Home";
import ProductListContainer from "./components/products/productList/ProductListContainer";
import ProductDetailContainer from "./components/products/productDetail/ProductDetailContainer";
import AdminDashboard from "./components/admin/dashboard/AdminDashboard";
import CategoryListContainer from "./components/admin/categories/CategoryListContainer";
import AdminProductListContainer from "./components/admin/products/ProductListContainer";
import { CartProvider } from "./contexts/CartContext";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import UserLayout from "./components/users/UserLayout";
import Logout from "./pages/auth/Logout";
import Registro from "./pages/auth/Registro";
import Login from "./pages/auth/Login";
import Profile from "./pages/profile/Profile";
import Orders from "./pages/profile/Orders";
import Favorites from "./pages/profile/Favorites";
import { FavoriteProvider } from "./contexts/FavoriteContext";
import PrivacyPolicy from "./pages/PrivacyPolicy";
import TiendaFisica from "./pages/TiendaFisica"
import CartPage from "./pages/CartPage"
import SobreNosotros from "./pages/SobreNosotros"
import ResetearContrasenia from "./pages/auth/ResetearContrasenia";
import RecuperarContrasenia from "./pages/auth/RecuperarContrasenia";
import Contact from "./pages/Contact";
import NotFound from './components/NotFound'
import AdminUserListContainer from "./components/admin/users/AdminUserListContainer";
import MessageListContainer from "./components/admin/messages/MessageListContainer";
import UserAddresses from "./components/users/addresses/UserAddresses";
import ConfirmEmail from "./components/common/ConfirmEmail";
import RegistrationSuccess from "./components/common/RegistrationSuccess";
import CheckoutPage from "./pages/CheckoutPage";
>>>>>>> FrontEnd
function App() {
  const isAuthenticated = true; // Cambia esto según tu lógica de autenticación

  return (
    <CartProvider>
      <Router>
        <Header />
        <Main>
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/products" element={<ProductListContainer />} />
            <Route path="/products/:id" element={<ProductDetailContainer />} />

<<<<<<< HEAD
            <Route
              path="/admin/*"
              element={
                isAuthenticated ? <AdminDashboard /> : <Navigate to="/" replace />
              }
            >
              <Route path="categories" element={<CategoryListContainer />} />
              <Route path="products" element={<AdminProductListContainer />} />
            </Route>
          </Routes>
        </Main>
        <Footer />
      </Router>
      <ToastContainer />
=======
              {/* Rutas de usuario */}
              <Route path="/profile" element={<UserLayout />}>
                <Route index element={<Profile />} />
                <Route path="orders" element={<Orders />} />
                <Route path="favorites" element={<Favorites />} />
                <Route path="addresses" element={<UserAddresses />}/>
                
              </Route>

              {/* Ruta protegida para el admin */}
              <Route path="/admin/*" element={<AdminDashboard />}>
                {/* Subrutas del dashboard */}
                <Route path="categories" element={<CategoryListContainer />} />
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
              <Route path="/resetearContrasenia" element={<ResetearContrasenia />}  />
              <Route path="/recuperarContrasenia" element={<RecuperarContrasenia />}  />
              <Route path="/register" element={<Registro />} />
              <Route path="/logout" element={<Logout />} />
              <Route path="/sobre-nosotros" element={<SobreNosotros />} />
              <Route path="/tienda-fisica" element={<TiendaFisica />} />
              <Route path="/contacto" element={<Contact />} />
              <Route path="/politica-de-privacidad" element={< PrivacyPolicy/>} />
              <Route path="/authentication/confirm-email" element={<ConfirmEmail />} />
              <Route path="/registration-success" element={<RegistrationSuccess />} />
              <Route path="/checkout" element={<CheckoutPage />} />
              <Route path="*" element={<NotFound />} /> 
            </Routes>
          </Main>
          <Footer />
        </Router>

        <ToastContainer />
      </FavoriteProvider>
>>>>>>> FrontEnd
    </CartProvider>
  );
}

export default App;
