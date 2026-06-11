import { Navigate } from 'react-router-dom';
import { getCurrentUser } from '../../store/authStore';
import './ProfilePage.css';

export const ProfilePage = () => {
  const user = getCurrentUser();

  if (!user) {
    return <Navigate to="/auth" replace />;
  }

  return (
    <div className="profile">
      <div className="profile__container">
        <h1 className="profile__title">Профиль</h1>
        
        <div className="profile__card">
          <p className="profile__placeholder">
            Страница профиля в разработке...
          </p>
          <p className="profile__info">
            Здесь будет информация о пользователе:
          </p>
          <ul className="profile__list">
            <li>📧 Email: {user.email}</li>
            <li>📱 Телефон: {user.phone}</li>
            <li>👤 ФИО: {user.fullName || 'Не заполнено'}</li>
            <li>🚛 Тип: {user.userType === 'client' ? 'Клиент' : user.userType === 'carrier' ? 'Перевозчик' : 'Не указан'}</li>
          </ul>
          <button
            className="profile__back"
            onClick={() => window.location.href = '/'}
          >
            На главную
          </button>
        </div>
      </div>
    </div>
  );
};