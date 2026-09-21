import { useOutletContext } from "react-router-dom";
import fakeOrders from "../../data/fakeOrder";
import UserOrderListContainer from "../../components/users/Orders/UserOrderListContainer";

const Orders = () => {
  const { user: currentUser } = useOutletContext();
  const orders = fakeOrders.filter((order) => order.userId === currentUser.id);

  return (
    <div className="orders-page">
      <div className="title">
        <h1>Mis Pedidos</h1>
      </div>
      <UserOrderListContainer orders={orders} />
    </div>
  );
};

export default Orders;
