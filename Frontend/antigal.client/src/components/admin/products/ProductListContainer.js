import React from 'react';
 import AdminProductList from './ProductList';
import AdminNav from '../AdminNav';

const AdminProductListContainer=()=>{
    return(
    <div className="admi-page">
        <AdminNav/>
        <div className='new-btn'>
            <button> + Nuevo Producto </button>
        </div>
        <AdminProductList />
    </div>
);} 
export default AdminProductListContainer;