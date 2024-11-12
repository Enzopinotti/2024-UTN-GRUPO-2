// src/components/common/ThemeSlider.js

import React, { useContext } from 'react';
import { ThemeContext } from '../../contexts/ThemeContext';

const ThemeSlider = () => {
  const { theme, toggleTheme } = useContext(ThemeContext);

  return (
    <div className="theme-slider">
      <input
        type="checkbox"
        id="theme-toggle"
        checked={theme === 'dark'}
        onChange={toggleTheme}
      />
      <label htmlFor="theme-toggle" className="slider">
        <span className="slider-icon">
          {theme === 'dark' ? '🌙' : '☀️'}
        </span>
      </label>
    </div>
  );
};

export default ThemeSlider;
