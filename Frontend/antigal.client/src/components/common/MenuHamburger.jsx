// src/components/common/MenuHamburger.js
const MenuHamburger = ({ onClick }) => {
  return (
    <div onClick={onClick}> 
      <img className='hamburgesa' src='/images/hamburgesa.svg' alt='menu de hamburguesa' />
    </div>
  );
}

export default MenuHamburger;