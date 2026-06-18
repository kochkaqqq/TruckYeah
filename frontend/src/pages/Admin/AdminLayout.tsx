import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useModeratorStore } from '../../store/moderatorStore';
import './Admin.css';

const navItems = [
  { to: '/admin/users', label: 'Пользователи', icon: 'П' },
  { to: '/admin/cargos', label: 'Грузы', icon: 'Г' },
  { to: '/admin/trucks', label: 'Машины', icon: 'М' },
];

export const AdminLayout = () => {
  const navigate = useNavigate();
  const { logout, isBusy } = useModeratorStore();

  const handleLogout = async () => {
    await logout();
    navigate('/admin/login', { replace: true });
  };

  return (
    <div className="admin-shell">
      <aside className="admin-sidebar">
        <div className="admin-sidebar__brand">
          <span className="admin-logo">truckyeah</span>
          <span className="admin-logo__badge">admin</span>
        </div>

        <nav className="admin-nav" aria-label="Разделы модератора">
          {navItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) =>
                `admin-nav__item ${isActive ? 'admin-nav__item--active' : ''}`
              }
            >
              <span className="admin-nav__icon">{item.icon}</span>
              {item.label}
            </NavLink>
          ))}
        </nav>

        <button
          className="admin-sidebar__logout"
          onClick={handleLogout}
          disabled={isBusy}
        >
          Выйти
        </button>
      </aside>

      <main className="admin-main">
        <header className="admin-topbar">
          <div>
            <p className="admin-eyebrow">TruckYeah</p>
            <strong>Модерация платформы</strong>
          </div>
          <span className="admin-topbar__status">
            <i />
            Сервис доступен
          </span>
        </header>
        <Outlet />
      </main>
    </div>
  );
};
