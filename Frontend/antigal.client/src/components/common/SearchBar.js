import React, { useState } from "react";
import LupaWidget from "./LupaWidget";

const SearchBar = ({isVisible, onClose,isMobile  }) => {
  const [searchTerm, setSearchTerm] = useState("");

  const items = [
    "Avena",
    "Barritas de cereal",
    "Leche vegetal",
    "Producto sin tacc",
  ];

  const handleSearch = () => {
       console.log("Searching for:", searchTerm);
   };


  const handleSearchChange = (e) => {
    const value = e.target.value;
    setSearchTerm(value);
  };

  const handleClearSearch = () => {
    setSearchTerm("");
  };

  return (
    <div className={`search-bar-container`}>
      <div className={`toggle-container ${isVisible? "visible" : "hidden"}`}>
        <div className="top-section">
          <h2>¿Podemos ayudarte?</h2>
          <button className="close-searchbar" onClick={onClose}>
            <i className="fa-solid fa-x"></i>
          </button>
        </div>

        <div className="search-container">
          <input
            type="text"
            placeholder="Buscar producto"
            value={searchTerm}
            onChange={handleSearchChange}
          />
          <button className="clear-searchbar" onClick={handleClearSearch}>
            <i class="fa-solid fa-circle-xmark"></i>
          </button>
          <LupaWidget onClick={handleSearch}/>
        </div>


        <div className="dropdown">
          <h4>Lo más buscado</h4>
          <ul>
            {items.map((item, index) => (
              <li key={index} onClick={() => setSearchTerm(item)}>
                {index + 1} {item}
              </li>
            ))}
          </ul>
        </div>
      </div>
    </div>
  );
};
export default SearchBar;
