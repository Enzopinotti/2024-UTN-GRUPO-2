import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Header from './components/layout/Header';
import Main from './components/layout/Main';
import Footer from './components/layout/Footer';
import Home from './components/Home';
import ProductListContainer from './components/products/productList/ProductListContainer';
import ProductDetail from './components/products/productDetail/ProductDetail';
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
