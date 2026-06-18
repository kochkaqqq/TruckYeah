import { useState, type FormEvent } from 'react';
import { Navigate, useLocation, useNavigate } from 'react-router-dom';
import { useModeratorStore } from '../../store/moderatorStore';
import './Admin.css';

export const AdminLoginPage = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { isAuthenticated, isBusy, login } = useModeratorStore();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  if (isAuthenticated) return <Navigate to="/admin/users" replace />;

  const handleSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setError('');
    try {
      await login(email.trim(), password);
      const destination =
        (location.state as { from?: { pathname?: string } } | null)?.from?.pathname ||
        '/admin/users';
      navigate(destination, { replace: true });
    } catch (loginError) {
      setError(loginError instanceof Error ? loginError.message : 'Не удалось войти.');
    }
  };

  return (
    <div className="admin-login">
      <div className="admin-login__brand">
        <span className="admin-logo">truckyeah</span>
        <span className="admin-logo__badge">moderator</span>
      </div>

      <form className="admin-login__card" onSubmit={handleSubmit}>
        <div>
          <p className="admin-eyebrow">Панель управления</p>
          <h1>Вход модератора</h1>
          <p className="admin-muted">Используйте служебную учётную запись.</p>
        </div>

        <label className="admin-field">
          <span>Email</span>
          <input
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            placeholder="moderator@example.com"
            autoComplete="username"
            required
          />
        </label>

        <label className="admin-field">
          <span>Пароль</span>
          <input
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            placeholder="Введите пароль"
            autoComplete="current-password"
            required
          />
        </label>

        {error && <div className="admin-alert admin-alert--error">{error}</div>}

        <button className="admin-button admin-button--primary" disabled={isBusy}>
          {isBusy ? 'Входим…' : 'Войти'}
        </button>
      </form>
    </div>
  );
};
