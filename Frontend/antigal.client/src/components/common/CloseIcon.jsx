const CloseIcon = ({ onClick }) => {
  return (
    <div className='close-icon' onClick={onClick}>
      <img className='closeIcon' src="/icons/cruz.svg" alt="Cerrar menú" />
    </div>
  );
};

export default CloseIcon;