import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../store/authStore';
import { api, UserType } from '../../api/client';
import { Toast } from '../../components/ui/Toast';
import './ProfilePage.css';

type ToastType = 'success' | 'error' | 'info';

interface ToastData {
  message: string;
  type: ToastType;
}

export const ProfileEditPage = () => {
  const { user, isAuthenticated, setCurrentUser } = useAuthStore();
  const navigate = useNavigate();

  const [toast, setToast] = useState<ToastData | null>(null);
  const [isSaving, setIsSaving] = useState(false);

  // Локальное состояние формы
  const [name, setName] = useState(user?.name || '');
  const [surname, setSurname] = useState(user?.surname || '');
  const [email, setEmail] = useState(user?.email || '');
  const [phone, setPhone] = useState(user?.phone || '');
  const [city, setCity] = useState(user?.city || '');
  const [company, setCompany] = useState(user?.company || '');

  if (!isAuthenticated || !user) {
    navigate('/auth');
    return null;
  }

  const showToast = (message: string, type: ToastType) => {
    setToast({ message, type });
  };

  const getUserTypeLabel = (type?: any) => {
    if (type === UserType.Business || type === 'business' || type === 0) return 'Компания';
    if (type === UserType.Private || type === 'private' || type === 1) return 'Частное лицо';
    if (type === UserType.Individual || type === 'individual' || type === 2) return 'ИП';
    return 'Не указан';
  };

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!name.trim()) {
      showToast('Введите имя', 'error');
      return;
    }

    if (!surname.trim()) {
      showToast('Введите фамилию', 'error');
      return;
    }

    if (!email.trim()) {
      showToast('Введите email', 'error');
      return;
    }

    setIsSaving(true);

    try {
      const updatedUser = await api.users.updateMe({
        name: name.trim(),
        surname: surname.trim(),
        email: email.trim(),
        phone: phone.trim(),
        postcode: user.postcode || '000000',
        city: city.trim() || undefined,
        companyName: company.trim() || undefined,
        avatarLink: user.avatarLink,
      });
      setCurrentUser(updatedUser);

      showToast('Профиль успешно обновлён!', 'success');
      
      setTimeout(() => {
        navigate('/profile');
      }, 1000);
    } catch (error) {
      console.error('Ошибка сохранения:', error);
      showToast('Ошибка сохранения профиля', 'error');
    } finally {
      setIsSaving(false);
    }
  };

  const handleCancel = () => {
    navigate('/profile');
  };

  return (
    <div className="profile">
      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}

      <div className="profile__container">
        <h1 className="profile__title">Редактирование профиля</h1>
        
        <form className="profile__card profile__card--form" onSubmit={handleSave}>
          <div className="profile__form-grid">
            <div className="profile__form-section">
              <h2 className="profile__section-title">Основная информация</h2>
              
              <div className="profile__field">
                <label className="profile__label">Фамилия *</label>
                <input
                  type="text"
                  className="profile__input"
                  value={surname}
                  onChange={(e) => setSurname(e.target.value)}
                  placeholder="Иванов"
                  required
                />
              </div>

              <div className="profile__field">
                <label className="profile__label">Имя *</label>
                <input
                  type="text"
                  className="profile__input"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  placeholder="Иван"
                  required
                />
              </div>

              <div className="profile__field">
                <label className="profile__label">Тип пользователя</label>
                <div className="profile__input profile__input--readonly">
                  {getUserTypeLabel(user.userType)}
                </div>
              </div>
            </div>

            <div className="profile__form-section">
              <h2 className="profile__section-title">Контактные данные</h2>
              
              <div className="profile__field">
                <label className="profile__label">Электронная почта *</label>
                <input
                  type="email"
                  className="profile__input"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="example@ati.su"
                  required
                />
              </div>

              <div className="profile__field">
                <label className="profile__label">Телефон</label>
                <input
                  type="tel"
                  className="profile__input"
                  value={phone}
                  onChange={(e) => setPhone(e.target.value)}
                  placeholder="+7 (999) 999-99-99"
                />
              </div>
            </div>

            <div className="profile__form-section">
              <h2 className="profile__section-title">Дополнительно</h2>
              
              <div className="profile__field">
                <label className="profile__label">Город</label>
                <input
                  type="text"
                  className="profile__input"
                  value={city}
                  onChange={(e) => setCity(e.target.value)}
                  placeholder="Москва"
                />
              </div>

              <div className="profile__field">
                <label className="profile__label">Компания</label>
                <input
                  type="text"
                  className="profile__input"
                  value={company}
                  onChange={(e) => setCompany(e.target.value)}
                  placeholder='ООО "Компания"'
                />
              </div>
            </div>
          </div>

          <div className="profile__form-actions">
            <button
              type="submit"
              className="profile__btn profile__btn--primary"
              disabled={isSaving}
            >
              {isSaving ? 'Сохранение...' : 'Сохранить'}
            </button>
            
            <button
              type="button"
              className="profile__btn profile__btn--secondary"
              onClick={handleCancel}
            >
              Отмена
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};
