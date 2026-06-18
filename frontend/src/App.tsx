import { BrowserRouter, Navigate, Routes, Route } from 'react-router-dom';
import { useEffect } from 'react';
import { Header } from './components/layout/Header';
import { ProtectedRoute } from './components/ProtectedRoute';
import { HomePage } from './pages/Home/HomePage';
import { AuthPage } from './pages/Auth/AuthPage';
import { ProfilePage } from './pages/Profile/ProfilePage';
import { ProfileEditPage } from './pages/Profile/ProfileEditPage';
import { ChatsPage } from './pages/Chats/ChatsPage';
import { ChatPage } from './pages/Chats/ChatPage';
import { CargoPage } from './pages/Cargo/CargoPage';
import { CargoAddPage } from './pages/Cargo/CargoAddPage';
import { CargoDetailPage } from './pages/Cargo/CargoDetailPage';
import { VehiclesPage } from './pages/Vehicles/VehiclesPage';
import { VehiclesAddPage } from './pages/Vehicles/VehiclesAddPage';
import { VehiclesDetailPage } from './pages/Vehicles/VehiclesDetailPage';
import { MyListingPage } from './pages/MyListing/MyListingPage';
import { TradesPage } from './pages/Trades/TradesPage'; 
import { OrdersPage } from './pages/Orders/OrdersPage';
import { NotFoundPage } from './pages/NotFound/NotFoundPage';
import { api } from './api/client';
import { useAuthStore } from './store/authStore';
import { AdminLoginPage } from './pages/Admin/AdminLoginPage';
import { AdminLayout } from './pages/Admin/AdminLayout';
import { AdminUsersPage } from './pages/Admin/AdminUsersPage';
import { AdminListingsPage } from './pages/Admin/AdminListingsPage';
import { AdminProtectedRoute } from './components/admin/AdminProtectedRoute';

function App() {
  const { isInitialized, setCurrentUser, markInitialized, logout } = useAuthStore();

  useEffect(() => {
    let active = true;

    const initializeAuth = async () => {
      if (!localStorage.getItem('authToken')) {
        markInitialized();
        return;
      }

      try {
        const user = await api.users.getMe();
        if (active) {
          setCurrentUser(user);
        }
      } catch {
        if (active) {
          logout();
        }
      }
    };

    void initializeAuth();
    return () => {
      active = false;
    };
  }, [logout, markInitialized, setCurrentUser]);

  if (!isInitialized) {
    return <div className="app-loading">Загрузка...</div>;
  }

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/admin/login" element={<AdminLoginPage />} />
        <Route
          path="/admin"
          element={
            <AdminProtectedRoute>
              <AdminLayout />
            </AdminProtectedRoute>
          }
        >
          <Route index element={<Navigate to="/admin/users" replace />} />
          <Route path="users" element={<AdminUsersPage />} />
          <Route path="cargos" element={<AdminListingsPage kind="cargo" />} />
          <Route path="trucks" element={<AdminListingsPage kind="truck" />} />
        </Route>

        <Route
          path="*"
          element={
            <div className="app-layout">
              <Header />
              <main className="app-main">
                <Routes>
            {/* Публичные маршруты */}
            <Route path="/" element={<HomePage />} />
            <Route path="/auth" element={<AuthPage />} />
            <Route path="/cargo" element={<CargoPage />} />
            <Route path="/cargo/:id" element={<CargoDetailPage />} />
            <Route path="/vehicles" element={<VehiclesPage />} />
            <Route path="/vehicles/:id" element={<VehiclesDetailPage />} />

            {/* Защищённые маршруты */}
            <Route path="/cargo/add" element={<ProtectedRoute><CargoAddPage /></ProtectedRoute>} />
            <Route path="/cargo/:id/edit" element={<ProtectedRoute><CargoAddPage /></ProtectedRoute>} />
            <Route path="/vehicles/add" element={<ProtectedRoute><VehiclesAddPage /></ProtectedRoute>} />
            <Route path="/vehicles/:id/edit" element={<ProtectedRoute><VehiclesAddPage /></ProtectedRoute>} />
            <Route path="/my-listing" element={<ProtectedRoute><MyListingPage /></ProtectedRoute>} />
            <Route path="/trades" element={<ProtectedRoute><TradesPage /></ProtectedRoute>} /> {/* ✅ Добавь эту строку */}
            <Route path="/profile" element={<ProtectedRoute><ProfilePage /></ProtectedRoute>} />
            <Route path="/profile/edit" element={<ProtectedRoute><ProfileEditPage /></ProtectedRoute>} />
            <Route path="/chats" element={<ProtectedRoute><ChatsPage /></ProtectedRoute>} />
            <Route path="/chats/:id" element={<ProtectedRoute><ChatPage /></ProtectedRoute>} />
            <Route path="*" element={<NotFoundPage />} />
            
            {/* Заглушки */}
            <Route path="/my-orders" element={<ProtectedRoute><OrdersPage /></ProtectedRoute>} />
            <Route path="/cargo/search" element={<Navigate to="/cargo" replace />} />
            <Route path="/vehicles/search" element={<Navigate to="/vehicles" replace />} />
                </Routes>
              </main>
            </div>
          }
        />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
