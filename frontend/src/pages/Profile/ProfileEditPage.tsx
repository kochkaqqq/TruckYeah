import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../store/authStore';
import { UserType } from '../../api/client';
import './ProfilePage.css';

export const ProfilePage = () => {
  const { user, isAuthenticated } = useAuthStore();
  const navigate = useNavigate();

  if (!isAuthenticated || !user) {
    navigate('/auth');
    return null;
  }

  const getUserTypeLabel = (type?: any) => {
    if (type === UserType.Business || type === 'business' || type === 0) return 'Компания';
    if (type === UserType.Private || type === 'private' || type === 1) return 'Частное лицо';
    if (type === UserType.Individual || type === 'individual' || type === 2) return 'ИП';
    return 'Не указан';
  };

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
            <li>📱 Телефон: {user.phone || 'Не указан'}</li>
            <li>👤 ФИО: {user.fullName || 'Не заполнено'}</li>
            <li>🚛 Тип: {getUserTypeLabel(user.userType)}</li>
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