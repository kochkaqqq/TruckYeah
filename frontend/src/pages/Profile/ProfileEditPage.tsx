import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../store/authStore';
import './ProfilePage.css';

export const ProfilePage = () => {
  const { user, isAuthenticated } = useAuthStore();
  const navigate = useNavigate();

  if (!isAuthenticated || !user) {
    navigate('/auth');
    return null;
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
            onClick={() => navigate('/')}
          >
            На главную
          </button>
        </div>
      </div>
    </div>
  );
};