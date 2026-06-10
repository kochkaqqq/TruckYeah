import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Header } from './components/layout/Header';
import { HomePage } from './pages/Home/HomePage';
import { AuthPage } from './pages/Auth/AuthPage';
import { ProfilePage } from './pages/Profile/ProfilePage';

const PlaceholderPage = ({ title }: { title: string }) => (
  <div style={{
    padding: '100px 40px',
    color: 'white',
    textAlign: 'center',
    backgroundColor: 'var(--color-bg-primary)',
    minHeight: '100vh'
  }}>
    <h1>Страница: {title}</h1>
    <p>Здесь будет контент для {title}</p>
    <a href="/" style={{ color: 'var(--color-accent)', marginTop: '20px', display: 'inline-block' }}>
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
            <Route path="/" element={<HomePage />} />
            <Route path="/auth" element={<AuthPage />} />
            <Route path="/profile" element={<ProfilePage />} />
            <Route path="/cargo" element={<PlaceholderPage title="Грузы" />} />
            <Route path="/vehicles" element={<PlaceholderPage title="Машины" />} />
            <Route path="/my-orders" element={<PlaceholderPage title="Ваши заказы" />} />
            <Route path="/chats" element={<PlaceholderPage title="Чаты" />} />
            <Route path="/cargo/add" element={<PlaceholderPage title="Добавить груз" />} />
            <Route path="/cargo/search" element={<PlaceholderPage title="Поиск грузов" />} />
            <Route path="/vehicles/add" element={<PlaceholderPage title="Добавить машину" />} />
            <Route path="/vehicles/search" element={<PlaceholderPage title="Поиск машин" />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}

export default App;