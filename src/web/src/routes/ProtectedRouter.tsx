import { useAuth } from "@/contexts/AuthContext";
import { Navigate, Outlet } from "react-router";

const ProtectedRouter = () => {
  const { isLoggedIn } = useAuth();

  return isLoggedIn ? <Outlet /> : <Navigate to="/login" />;
};

export default ProtectedRouter;
