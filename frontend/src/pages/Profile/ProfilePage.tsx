import { useState } from 'react';
import { Navigate, useNavigate } from 'react-router-dom';
import { getCurrentUser, useAuthStore } from '../../store/authStore';
import './ProfilePage.css';

// Inline SVG заглушка для аватара
const AvatarPlaceholder = () => (
  <svg 
    xmlns="http://www.w3.org/2000/svg" 
    viewBox="0 0 300 300" 
    style={{ width: '100%', height: '100%' }}
  >
    <circle cx="150" cy="150" r="150" fill="#d1d5db"/>
    <circle cx="150" cy="110" r="50" fill="#f3f4f6"/>
    <ellipse cx="150" cy="260" rx="90" ry="70" fill="#f3f4f6"/>
  </svg>
);

export const ProfilePage = () => {
  const user = getCurrentUser();
  const navigate = useNavigate();
  const { logout } = useAuthStore();
  
  const [showImageModal, setShowImageModal] = useState(false);
  const [showLogoutModal, setShowLogoutModal] = useState(false);

  if (!user) {
    return <Navigate to="/auth" replace />;
  }

  const formatDate = (dateString?: string) => {
    if (!dateString) return new Date().toLocaleDateString('ru-RU');
    return new Date(dateString).toLocaleDateString('ru-RU');
  };

  const handleLogout = () => {
    localStorage.clear();
    sessionStorage.clear();
    logout();
    setShowLogoutModal(false);
    navigate('/');
  };

  return (
    <div className="profile">
      <div className="profile__container">
        <h1 className="profile__title">Профиль</h1>
        
        <div className="profile__card">
          <div className="profile__info-section">
            <div className="profile__name">
              {user.fullName || `${user.surname || ''} ${user.name || ''}`.trim() || 'Пользователь'}
            </div>
            
            <div className="profile__id">
              ID: {user.id ? user.id.substring(0, 8) : '--------'}
            </div>

            <div className="profile__details">
              {user.phone && (
                <div className="profile__detail-item">
                  <span className="profile__detail-icon">📱</span>
                  <span className="profile__detail-text">+{user.phone}</span>
                </div>
              )}
              
              {user.email && (
                <div className="profile__detail-item">
                  <span className="profile__detail-icon">📧</span>
                  <span className="profile__detail-text">{user.email}</span>
                </div>
              )}

              <div className="profile__detail-item">
                <span className="profile__detail-icon">📍</span>
                <span className="profile__detail-text">{user.city || 'г. Москва'}</span>
              </div>

              <div className="profile__detail-item">
                <span className="profile__detail-icon">🏢</span>
                <span className="profile__detail-text">{user.company || 'ООО "Компания"'}</span>
              </div>

              <div className="profile__detail-item">
                <span className="profile__detail-icon">✓</span>
                <span className="profile__detail-text profile__verified">
                  Подтверждённый профиль
                </span>
              </div>

              <div className="profile__detail-item">
                <span className="profile__detail-icon">📅</span>
                <span className="profile__detail-text">
                  Дата регистрации: {formatDate()}
                </span>
              </div>

              <div className="profile__detail-item">
                <span className="profile__detail-icon">⭐</span>
                <span className="profile__detail-text">
                  Рейтинг: <strong>5.0</strong> / 10
                </span>
                <div className="profile__rating-bar">
                  <div className="profile__rating-fill" style={{ width: '50%' }}></div>
                </div>
              </div>
            </div>

            <div className="profile__actions">
              <button
                className="profile__btn profile__btn--primary"
                onClick={() => navigate('/profile/edit')}
              >
                Изменить профиль
              </button>
              
              <button
                className="profile__btn profile__btn--secondary"
                onClick={() => setShowLogoutModal(true)}
              >
                Выйти
              </button>
            </div>
          </div>

          <div className="profile__photo-section">
            <div 
              className="profile__photo-wrapper"
              onClick={() => setShowImageModal(true)}
            >
              <AvatarPlaceholder />
              <div className="profile__photo-overlay">
                <span className="profile__photo-zoom">🔍</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      {showImageModal && (
        <div 
          className="profile__modal"
          onClick={() => setShowImageModal(false)}
        >
          <div 
            className="profile__modal-content"
            onClick={(e) => e.stopPropagation()}
          >
            <button 
              className="profile__modal-close"
              onClick={() => setShowImageModal(false)}
            >
              ✕
            </button>
            <div className="profile__modal-image-wrapper">
              <AvatarPlaceholder />
            </div>
          </div>
        </div>
      )}

      {showLogoutModal && (
        <div className="profile__modal">
          <div className="profile__modal-content profile__modal-content--small">
            <h2 className="profile__modal-title">Выход из аккаунта</h2>
            <p className="profile__modal-text">
              Вы действительно хотите выйти из системы?
            </p>
            <div className="profile__modal-actions">
              <button
                className="profile__btn profile__btn--secondary"
                onClick={() => setShowLogoutModal(false)}
              >
                Отмена
              </button>
              <button
                className="profile__btn profile__btn--danger"
                onClick={handleLogout}
              >
                Выйти
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};