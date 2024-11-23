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


<<<<<<< HEAD
=======
    try {
      const response = await fetch(
        `https://www.antigal.somee.com/api/Product/getProductByTitle/${encodeURIComponent(searchTerm)}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
          },
        }
      );

      if (!response.ok) {
        throw new Error(`Error: ${response.status}`);
      }

      const data = await response.json();

      if (data.isSuccess) {
        const productos = data.data.$values; // Verifica esta estructura
        console.log(data);
        setSearchResults(productos);
        if (productos.length === 0) {
          toast.info("No se encontraron productos con ese término.");
        }
      } else {
        setError(data.message || "Error al buscar productos.");
        toast.error(data.message || "Error al buscar productos.");
      }
    } catch (err) {
      console.error("Error al buscar productos:", err);
      setError("Error al buscar productos. Inténtalo de nuevo más tarde.");
      toast.error("Error al buscar productos. Inténtalo de nuevo más tarde.");
    } finally {
      setLoading(false);
    }
  };

  // Función para manejar cambios en el input
>>>>>>> FrontEnd
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
