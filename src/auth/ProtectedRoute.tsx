import { Navigate, Outlet } from "react-router-dom";

// Define an authentication function (Replace this with actual authentication logic)
const isAuthenticated = (): boolean => {
  return localStorage.getItem("authToken") !== null; // Example: Check if auth token exists
};

// Protected Route Component
const ProtectedRoute: React.FC = () => {
  return isAuthenticated() ? <Outlet /> : <Navigate to="/login" replace />;
};

export default ProtectedRoute;
