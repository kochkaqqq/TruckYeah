import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../store/authStore';
import { isValidEmail, isValidPhone, isValidPassword } from '../../utils/validation';
import { formatPhone, getDigitsFromPhone } from '../../utils/phoneMask';
import { Toast } from '../../components/ui/Toast';
import './AuthPage.css';

type AuthMode = 'select' | 'login' | 'register';
type ToastType = 'success' | 'error' | 'info';

interface ToastData {
  message: string;
  type: ToastType;
}

export const AuthPage = () => {
  const [mode, setMode] = useState<AuthMode>('select');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [toast, setToast] = useState<ToastData | null>(null);

  const { login, register } = useAuthStore();
  const navigate = useNavigate();

  const showToast = (message: string, type: ToastType) => {
    setToast({ message, type });
  };

  const handlePhoneChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const formatted = formatPhone(e.target.value);
    setPhone(formatted);
  };

  const handleLoginPhoneChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    if (value.includes('@')) {
      setEmail(value);
      setPhone('');
    } else {
      const formatted = formatPhone(value);
      setPhone(formatted);
      setEmail('');
    }
  };

  const handleLogin = (e: React.FormEvent) => {
    e.preventDefault();

    const identifier = email || getDigitsFromPhone(phone);
    if (!identifier) {
      showToast('Введите email или номер телефона', 'error');
      return;
    }

    if (email && !isValidEmail(email)) {
      showToast('Неверный формат email', 'error');
      return;
    }

    if (phone && !isValidPhone(phone)) {
      showToast('Неверный формат телефона', 'error');
      return;
    }

    if (!password) {
      showToast('Введите пароль', 'error');
      return;
    }

    if (!isValidPassword(password)) {
      showToast('Пароль должен содержать минимум 6 символов', 'error');
      return;
    }

    login(email, getDigitsFromPhone(phone));
    showToast('Вход выполнен успешно!', 'success');
    setTimeout(() => navigate('/'), 1000);
  };

  const handleRegister = (e: React.FormEvent) => {
    e.preventDefault();

    if (!email) {
      showToast('Введите email', 'error');
      return;
    }

    if (!isValidEmail(email)) {
      showToast('Неверный формат email', 'error');
      return;
    }

    if (!phone) {
      showToast('Введите номер телефона', 'error');
      return;
    }

    if (!isValidPhone(phone)) {
      showToast('Неверный формат телефона', 'error');
      return;
    }

    if (!password) {
      showToast('Введите пароль', 'error');
      return;
    }

    if (!isValidPassword(password)) {
      showToast('Пароль должен содержать минимум 6 символов', 'error');
      return;
    }

    if (password !== confirmPassword) {
      showToast('Пароли не совпадают', 'error');
      return;
    }

    register(email, getDigitsFromPhone(phone), password);
    showToast('Регистрация успешна! Заполните профиль', 'success');
    setTimeout(() => navigate('/profile/edit'), 1000);
  };

  return (
    <div className="auth">
      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}

      <div className="auth__container">
        <div className="auth__card">
          <div className="auth__tabs">
            <button
              className={`auth__tab ${mode === 'register' ? 'auth__tab--active' : ''}`}
              onClick={() => setMode('register')}
            >
              Регистрация
            </button>
            <button
              className={`auth__tab ${mode === 'login' ? 'auth__tab--active' : ''}`}
              onClick={() => setMode('login')}
            >
              Вход
            </button>
          </div>

          {mode === 'select' && (
            <div className="auth__select">
              <h2 className="auth__title">Войдите или зарегистрируйтесь</h2>
              <p className="auth__subtitle">
                Для доступа к профилю необходимо авторизоваться
              </p>
              <div className="auth__buttons">
                <button
                  className="auth__btn auth__btn--primary"
                  onClick={() => setMode('register')}
                >
                  Зарегистрироваться
                </button>
                <button
                  className="auth__btn auth__btn--secondary"
                  onClick={() => setMode('login')}
                >
                  Войти
                </button>
              </div>
            </div>
          )}

          {mode === 'register' && (
            <form className="auth__form" onSubmit={handleRegister}>
              <h2 className="auth__title">Введите данные для регистрации</h2>

              <div className="auth__field">
                <label className="auth__label">Электронная почта</label>
                <input
                  type="email"
                  className="auth__input"
                  placeholder="example@ati.su"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                />
              </div>

              <div className="auth__field">
                <label className="auth__label">Номер телефона</label>
                <input
                  type="tel"
                  className="auth__input"
                  placeholder="+7 (999) 999-99-99"
                  value={phone}
                  onChange={handlePhoneChange}
                />
              </div>

              <div className="auth__field">
                <label className="auth__label">Пароль</label>
                <input
                  type="password"
                  className="auth__input"
                  placeholder="Минимум 6 символов"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />
              </div>

              <div className="auth__field">
                <label className="auth__label">Подтвердите пароль</label>
                <input
                  type="password"
                  className="auth__input"
                  placeholder="Повторите пароль"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                />
              </div>

              <button type="submit" className="auth__btn auth__btn--primary">
                Зарегистрироваться
              </button>

              <button
                type="button"
                className="auth__link"
                onClick={() => setMode('login')}
              >
                Уже есть аккаунт? Войти
              </button>
            </form>
          )}

          {mode === 'login' && (
            <form className="auth__form" onSubmit={handleLogin}>
              <h2 className="auth__title">Введите данные для входа</h2>

              <div className="auth__field">
                <label className="auth__label">Электронная почта или телефон</label>
                <input
                  type="text"
                  className="auth__input"
                  placeholder="example@ati.su или +7 (999) 999-99-99"
                  value={email || phone}
                  onChange={handleLoginPhoneChange}
                />
              </div>

              <div className="auth__field">
                <label className="auth__label">Пароль</label>
                <input
                  type="password"
                  className="auth__input"
                  placeholder="Введите пароль"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />
              </div>

              <button type="submit" className="auth__btn auth__btn--primary">
                Войти
              </button>

              <button
                type="button"
                className="auth__link"
                onClick={() => setMode('register')}
              >
                Нет аккаунта? Зарегистрироваться
              </button>
            </form>
          )}
        </div>
      </div>
    </div>
  );
};