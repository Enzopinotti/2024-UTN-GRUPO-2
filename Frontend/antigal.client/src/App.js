import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Header from './components/layout/Header';
import Main from './components/layout/Main';
import Footer from './components/layout/Footer';
import Home from './pages/Home';  
import ProductListContainer from './components/products/productList/ProductListContainer';
import ProductDetail from './components/products/productDetail/ProductDetail';
import CategoryListContainer from './components/admin/categories/CategoryListContainer';
import AdminProductListContainer from './components/admin/products/ProductListContainer';
// import CartContainer from './components/carts/CartContainer';
// import CheckoutContainer from './components/checkout/CheckoutContainer';
// import Login from './components/auth/Login';

function App() {
  return (
    <Router>
      <Header />
      <Main>
        <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/products" element={<ProductListContainer />} />
            <Route path="/products/:id" element={<ProductDetail />} />
            <Route path="/admin/categories" element={<CategoryListContainer />} />
            <Route path="/admin/products" element={<AdminProductListContainer />} />
          {/* <Route path="/cart" element={<CartContainer />} /> */}
          {/* <Route path="/checkout" element={<CheckoutContainer />} /> */}
          {/* <Route path="/login" element={<Login />} /> */}
        </Routes>
      </Main>
      <Footer />
    </Router>
  );
}

export default App;
