import React, { useState } from "react";
import LupaWidget from "./LupaWidget";

const SearchBarMobile = () =>{
    const [searchTerm, setSearchTerm] = useState("");

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
return(
   <div className="search-container mobile">
          <input
            type="text"
            placeholder="Buscar producto"
            value={searchTerm}
            onChange={handleSearchChange}
          />
          <button className="clear-searchbar" onClick={handleClearSearch}>
            <i className="fa-solid fa-circle-xmark"></i>
          </button>
          <LupaWidget onClick={handleSearch}/>
        </div>
)
}
export default SearchBarMobile;