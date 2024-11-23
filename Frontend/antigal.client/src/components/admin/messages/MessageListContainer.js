import React, {useEffect, useState} from 'react'
import AdminNav from '../AdminNav'
import MessageList from './MessageList'
import Swal from 'sweetalert2';
import fakeMessages from '../../../data/fakeMessages';



const MessageListContainer = () => {
    const [messages, setMessages]  = useState([]);
    const useBackend = false;
    useEffect(()=>{
        const fetchMessages = async ()=>{
            try{
                if (useBackend) {
                const response = await fetch("https://localhost:7255/api/Contacto");
                if(!response.ok){
                    throw new Error('Error al obtener los datos del backend')
                }
                const data = await response.json();
                setMessages(data.$values);}
                else{
                    setMessages(fakeMessages);
                }
            }catch(error){console.error("Error al obtener mensajes:", error);
                Swal.fire("Error", "No se pudieron obtener los mensajes.", "error");
              }
        };
        fetchMessages();
    },[useBackend]);


    return (
    <div className="admin-page">
      <AdminNav/>
      <div className="content">
        <h2>Lista de mensajes</h2>
        <div className='message-list-container'>
            <MessageList messages={messages}/>
        </div>
      </div>
    </div>
  )
}

export default MessageListContainer
