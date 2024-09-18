import React, {useState} from 'react';
import AdminProductList from './ProductList';
import AdminNav from '../AdminNav';
import ProductForm from './ProductForm';

const AdminProductListContainer=()=>{
    const [showModal, setShowModal] = useState(false);
    const handleShowModal = () => {
      setShowModal(true);
    };
    const handleCloseModal = () => {
      setShowModal(false);
    };
    return(
    <div className="admi-page">
        <AdminNav/>
        <div className='new-btn'>
        <button onClick={handleShowModal}> + Nuevo Producto </button>
        </div>
        <ProductForm show={showModal} onClose={handleCloseModal} />

        <AdminProductList />
    </div>
);} 
export default AdminProductListContainer;