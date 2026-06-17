import { BrowserRouter, Routes, Route } from 'react-router-dom';
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
import { NotFoundPage } from './pages/NotFound/NotFoundPage';

const PlaceholderPage = ({ title }: { title: string }) => (
  <div style={{
    padding: '100px 40px',
    color: 'white',
    textAlign: 'center',
    backgroundColor: '#1a1a1e',
    minHeight: '100vh'
  }}>
    <h1>Страница: {title}</h1>
    <p>Здесь будет контент для {title}</p>
    <a href="/" style={{ color: '#666666', marginTop: '20px', display: 'inline-block' }}>
      ← Вернуться на главную
    </a>
  </div>
);

function App() {
  return (
    <BrowserRouter>
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
            <Route path="/vehicles/add" element={<ProtectedRoute><VehiclesAddPage /></ProtectedRoute>} />
            <Route path="/my-listing" element={<ProtectedRoute><MyListingPage /></ProtectedRoute>} />
            <Route path="/trades" element={<ProtectedRoute><TradesPage /></ProtectedRoute>} /> {/* ✅ Добавь эту строку */}
            <Route path="/profile" element={<ProtectedRoute><ProfilePage /></ProtectedRoute>} />
            <Route path="/profile/edit" element={<ProtectedRoute><ProfileEditPage /></ProtectedRoute>} />
            <Route path="/chats" element={<ProtectedRoute><ChatsPage /></ProtectedRoute>} />
            <Route path="/chats/:id" element={<ProtectedRoute><ChatPage /></ProtectedRoute>} />
            <Route path="*" element={<NotFoundPage />} />
            
            {/* Заглушки */}
            <Route path="/my-orders" element={<PlaceholderPage title="Ваши заказы" />} />
            <Route path="/cargo/search" element={<PlaceholderPage title="Поиск грузов" />} />
            <Route path="/vehicles/search" element={<PlaceholderPage title="Поиск машин" />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}

export default App;