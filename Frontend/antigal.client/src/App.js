// src/App.js
import React from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import Header from './components/layout/Header';
import Main from './components/layout/Main';
import Footer from './components/layout/Footer';
import Home from './pages/Home';
import ProductListContainer from './components/products/productList/ProductListContainer';
import ProductDetail from './components/products/productDetail/ProductDetail';
import AdminDashboard from './components/admin/dashboard/AdminDashboard';
import CategoryListContainer from './components/admin/categories/CategoryListContainer';
import AdminProductListContainer from './components/admin/products/ProductListContainer';
import { CartProvider } from './contexts/CartContext';

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
            <Route path="/products/:id" element={<ProductDetail />} />

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
    </CartProvider>
  );
}

export default App;
