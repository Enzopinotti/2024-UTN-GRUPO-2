// src/App.js
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
import Logout from "./pages/Logout";
import Registro from "./pages/Registro";
import Login from "./pages/Login";
import Profile from "./pages/Profile";
import ProtectedRoute from "./components/common/ProtectedRoute";
import SobreNosotros from './pages/SobreNosotros'; // Importa el nuevo componente
function App() {
  return (
    <CartProvider>
      <Router>
        <Header />
        <Main>
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/products" element={<ProductListContainer />} />
            <Route path="/products/:id" element={<ProductDetailContainer />} />

            {/* Ruta protegida para el perfil */}
            <Route
              path="/profile"
              element={
                <ProtectedRoute>
                  <UserLayout />
                </ProtectedRoute>
              }
            >
              <Route index element={<Profile />} />
              {/* 
              <Route path="/orders" element={<UserOrders />} />
              <Route path="/addresses" element={<UserAddresses />} /> */}
            </Route>
           
          
            

            {/* Ruta protegida para el admin */}
            <Route path="/admin/*" element={<AdminDashboard />}>
              {/* Subrutas del dashboard */}
              <Route path="categories" element={<CategoryListContainer />} />
              <Route path="products" element={<AdminProductListContainer />} />
            </Route>

            {/* Rutas de autenticación */}
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Registro />} />
            <Route path="/logout" element={<Logout />} />
            <Route path="/sobre-nosotros" element={<SobreNosotros />} />
          </Routes>
        </Main>
        <Footer />
      </Router>
      <ToastContainer />
    </CartProvider>
  );
}

export default App;
