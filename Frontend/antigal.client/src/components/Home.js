import React from 'react';
import Promotions from './Promotions';
import CategoryCarousel from './categories/CategoryCarousel';

const Home = () => {
  return (
    <div>
      <h1>Home</h1>
      <Promotions />
      <CategoryCarousel />
    </div>
  );
};

export default Home;