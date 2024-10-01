import React, {useState,useEffect} from "react";
import { useParams } from "react-router-dom";
import LoadingSVG from "../../common/LoadingSVG";
import ErrorAnimation from "../../common/ErrorAnimation";
import ProductDetail from './ProductDetail';

const ProductDetailContainer = () =>{
    const {id} =useParams();
    const[product,setProduct] =useState(null);
    const [loading, setLoading]=useState(true);
    const [error, setError]=useState(false);
   

    useEffect(()=>{
        const useBackend=false;
        const fetchURL= useBackend ? `http://localhost:5000/api/products/${id}`: `https://fakestoreapi.com/products/${id}`;
        console.log(fetchURL);

        fetch (fetchURL)
            .then(response => {
                if(!response.ok){
                    
                    throw new Error('Error al obtener el producto');
                    
                }
                return response.json();
            })
            .then (data =>{
    
                const loadedProduct ={
                    id: data.id,
                    name: data.title,
                    price: data.price,
                    imageUrl: data.image,
                    description: data.description,
                    category: data.category,
                    //setea un stock para cuando no se usa el backend porque fakestore no tiene
                    stock: useBackend ? data.stock : (data.stock || 10),
                };
                setProduct(loadedProduct);
                setLoading(false);
                setError(false);
            })
            .catch(()=>{
                setLoading(false);
                console.log('no encontre');  // Ya no está cargando
                setError(true);
            });
    },[id]);

    return(
        <div className="page-detail-container">
            {loading ? (
                <div className="loading-container">
                    <LoadingSVG/>    
                </div>
            ): error ? (
                <div className="error-container">
                    <ErrorAnimation />
                    <h2>Oops... Algo salió mal. Intenta de nuevo más tarde.</h2>
                </div>
            ):
            (
                product && <ProductDetail product={product} /> // Mostrar detalles del producto usando tu componente
              )}
        </div>
    );

};
export default ProductDetailContainer;