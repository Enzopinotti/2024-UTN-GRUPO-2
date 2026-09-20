// src/components/admin/dashboard/AdminDashboard.js
import { Outlet } from 'react-router-dom';

const AdminDashboard = () => {
  return (
    <div className="admin-dashboard">
      <Outlet />
    </div>
  );
};

export default AdminDashboard;
