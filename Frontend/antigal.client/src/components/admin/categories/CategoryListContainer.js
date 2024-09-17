import React from 'react';
import CategoryList from './CategoryList';
import AdminNav from '../AdminNav';

const CategoryListContainer=()=>{
    return(
    <div className="admi-page">
        <AdminNav/>
        <div className='new-btn'>
            <button> + Nueva Categoría </button>
        </div>
        <CategoryList/>
    </div>
);} 
export default CategoryListContainer;