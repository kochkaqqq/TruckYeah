import { Navigate, useLocation } from 'react-router-dom';
import { useModeratorStore } from '../../store/moderatorStore';

export const AdminProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const isAuthenticated = useModeratorStore((state) => state.isAuthenticated);
  const location = useLocation();

  if (!isAuthenticated) {
    return <Navigate to="/admin/login" state={{ from: location }} replace />;
  }

  return <>{children}</>;
};
