import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../store/authStore';
import { isValidEmail, isValidPhone, isValidPassword } from '../../utils/validation';
import { formatPhone, getDigitsFromPhone } from '../../utils/phoneMask';
import { parseJwt } from '../../utils/jwt';
import { Toast } from '../../components/ui/Toast';
import { api, UserType, Country } from '../../api/client';
import './AuthPage.css';

type AuthMode = 'select' | 'login' | 'register';
type ToastType = 'success' | 'error' | 'info';

interface ToastData {
  message: string;
  type: ToastType;
}

export const AuthPage = () => {
  const [mode, setMode] = useState<AuthMode>('select');
  const [loginIdentifier, setLoginIdentifier] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [name, setName] = useState('');
  const [surname, setSurname] = useState('');
  const [vatId, setVatId] = useState('');
  const [companyName, setCompanyName] = useState('');
  const [userType, setUserType] = useState<UserType>(1);
  const [selectedCountryId, setSelectedCountryId] = useState<string>('');
  const [countries, setCountries] = useState<Country[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [toast, setToast] = useState<ToastData | null>(null);

  const {  login } = useAuthStore();
  const navigate = useNavigate();

  useEffect(() => {
    const loadCountries = async () => {
      try {
        console.log('Загрузка стран...');
        const data = await api.countries.getAll();
        console.log('Страны загружены:', data);

        const normalizedCountries = data.map((country: any) => ({
          id: country.id,
          name:
            typeof country.name === 'object' && country.name !== null
              ? country.name.value
              : country.name,
        }));

        console.log('Нормализованные страны:', normalizedCountries);

        setCountries(normalizedCountries);
        if (normalizedCountries.length > 0) {
          setSelectedCountryId(normalizedCountries[0].id);
        }
      } catch (error) {
        console.error('Ошибка загрузки стран:', error);
        const fallbackCountries = [
          { id: '1', name: 'Russia' },
          { id: '2', name: 'Belarus' },
          { id: '3', name: 'Kazakhstan' },
        ];
        setCountries(fallbackCountries);
        setSelectedCountryId(fallbackCountries[0].id);
        showToast('Не удалось загрузить список стран', 'error');
      }
    };

    loadCountries();
  }, []);

  const showToast = (message: string, type: ToastType) => {
    setToast({ message, type });
  };

  const handlePhoneChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const formatted = formatPhone(e.target.value);
    setPhone(formatted);
  };

  const handleLoginPhoneChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setLoginIdentifier(e.target.value);
  };

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();

    const identifier = loginIdentifier.trim();
    if (!identifier) {
      showToast('Введите email или номер телефона', 'error');
      return;
    }

    const loginByEmail = identifier.includes('@');
    const loginEmail = loginByEmail ? identifier : '';
    const loginPhone = loginByEmail ? '' : getDigitsFromPhone(identifier);

    if (loginByEmail && !isValidEmail(loginEmail)) {
      showToast('Неверный формат email', 'error');
      return;
    }

    if (!loginByEmail && !isValidPhone(identifier)) {
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

    setIsLoading(true);
    try {
      const loginData: any = {
        password,
        userAgent: typeof navigator !== 'undefined' ? navigator.userAgent : '',
        ipAddress: '127.0.0.1',
        deviceId: 'browser-' + Date.now().toString(),
      };

      if (loginEmail) loginData.email = loginEmail;
      if (loginPhone) loginData.phone = loginPhone;

      console.log('Отправка логина:', loginData);

      const response = await api.users.login(loginData);
      console.log('Ответ логина:', response);

      // ✅ Сохраняем токены
      if (response.jwtToken) {
        localStorage.setItem('authToken', response.jwtToken);

        // ✅ Парсим JWT и извлекаем данные пользователя
        const userData = parseJwt(response.jwtToken);
        console.log('Данные из JWT:', userData);

        if (userData) {
          // Разбираем fullName на name и surname
          const fullName = userData.name || '';
          const nameParts = fullName.trim().split(' ');
          const userSurname = nameParts[0] || '';
          const userName = nameParts[1] || '';

          const user = {
            id: userData.id || 'unknown',
            email: userData.email || loginEmail,
            phone: userData.phone || loginPhone,
            name: userName,
            surname: userSurname,
            fullName: fullName,
            isProfileCompleted: true,
          };

          login(user); 
        }
      }

      if (response.refreshToken) {
        localStorage.setItem('refreshToken', response.refreshToken);
      }
      login(await api.users.getMe());

      showToast('Вход выполнен успешно!', 'success');
      setTimeout(() => navigate('/'), 1000);
    } catch (error: any) {
      console.error('Ошибка входа:', error);
      showToast(error.message || 'Ошибка входа. Проверьте данные', 'error');
    } finally {
      setIsLoading(false);
    }
  };

  const handleRegister = async (e: React.FormEvent) => {
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

    if (!name.trim()) {
      showToast('Введите имя', 'error');
      return;
    }

    if (!surname.trim()) {
      showToast('Введите фамилию', 'error');
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

    if (!selectedCountryId) {
      showToast('Выберите страну', 'error');
      return;
    }

    if (userType === UserType.Business && !vatId.trim()) {
      showToast('Для компании необходимо указать VAT ID', 'error');
      return;
    }

    if (userType === UserType.Business && !companyName.trim()) {
      showToast('Введите название компании', 'error');
      return;
    }

    setIsLoading(true);
    try {
      const registerData = {
        email,
        phone: getDigitsFromPhone(phone),
        password,
        userType,
        name: name.trim(),
        surname: surname.trim(),
        middleName: undefined,
        countryId: selectedCountryId,
        postcode: '000000',
        vatId: userType === UserType.Private ? undefined : vatId.trim() || undefined,
        companyName: userType === UserType.Business ? companyName.trim() : undefined,
        avatarLink: '',
        deviceId: 'browser-' + Date.now().toString(),
        userAgent: typeof navigator !== 'undefined' ? navigator.userAgent : '',
        ipAddress: '127.0.0.1',
      };

      console.log('Отправка регистрации:');
      console.log(JSON.stringify(registerData, null, 2));

      const response = await api.users.register(registerData);
      console.log('Ответ регистрации:', response);

      // ✅ Сохраняем токены
      if (response.jwtToken) {
        localStorage.setItem('authToken', response.jwtToken);

        // ✅ Парсим JWT и извлекаем данные пользователя
        const userData = parseJwt(response.jwtToken);
        console.log('Данные из JWT:', userData);

        if (userData) {
          const fullName = userData.name || '';
          const nameParts = fullName.trim().split(' ');
          const userSurname = nameParts[0] || '';
          const userName = nameParts[1] || '';

          login({
            id: userData.id || 'unknown',
            email: userData.email || email,
            phone: userData.phone || getDigitsFromPhone(phone),
            name: userName,
            surname: userSurname,
            fullName: fullName,
            isProfileCompleted: true,
          });
        }
      }

      if (response.refreshToken) {
        localStorage.setItem('refreshToken', response.refreshToken);
      }
      login(await api.users.getMe());

      showToast('Регистрация успешна!', 'success');
      setTimeout(() => navigate('/'), 1000);
    } catch (error: any) {
      console.error('Ошибка регистрации:', error);
      showToast(
        error.message || 'Ошибка регистрации. Возможно, пользователь уже существует',
        'error'
      );
    } finally {
      setIsLoading(false);
    }
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
                  required
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
                  required
                />
              </div>

              <div className="auth__field-row">
                <div className="auth__field">
                  <label className="auth__label">Фамилия</label>
                  <input
                    type="text"
                    className="auth__input"
                    placeholder="Ivanov"
                    value={surname}
                    onChange={(e) => setSurname(e.target.value)}
                    required
                  />
                </div>

                <div className="auth__field">
                  <label className="auth__label">Имя</label>
                  <input
                    type="text"
                    className="auth__input"
                    placeholder="Ivan"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    required
                  />
                </div>
              </div>

              <div className="auth__field">
                <label className="auth__label">Тип пользователя</label>
                <select
                  className="auth__input"
                  value={userType}
                  onChange={(e) => setUserType(Number(e.target.value))}
                  required
                >
                  <option value={1}>Частное лицо</option>
                  <option value={2}>Индивидуальный предприниматель</option>
                  <option value={0}>Компания</option>
                </select>
              </div>

              <div className="auth__field">
                <label className="auth__label">Страна</label>
                <select
                  className="auth__input"
                  value={selectedCountryId}
                  onChange={(e) => setSelectedCountryId(e.target.value)}
                  required
                >
                  {countries.length === 0 ? (
                    <option value="">Загрузка...</option>
                  ) : (
                    countries.map((country) => (
                      <option key={country.id} value={country.id}>
                        {typeof country.name === 'string'
                          ? country.name
                          : (country.name as any)?.value || 'Unknown'}
                      </option>
                    ))
                  )}
                </select>
              </div>

              {userType !== UserType.Private && (
                <div className="auth__field">
                  <label className="auth__label">
                    {userType === UserType.Business
                      ? 'VAT ID (обязательно)'
                      : 'VAT ID (опционально)'}
                  </label>
                  <input
                    type="text"
                    className="auth__input"
                    placeholder="DE123456789"
                    value={vatId}
                    onChange={(e) => setVatId(e.target.value)}
                    required={userType === UserType.Business}
                  />
                </div>
              )}

              {userType === UserType.Business && (
                <div className="auth__field">
                  <label className="auth__label">Название компании</label>
                  <input
                    type="text"
                    className="auth__input"
                    placeholder='ООО "Транспорт"'
                    value={companyName}
                    onChange={(e) => setCompanyName(e.target.value)}
                    required
                  />
                </div>
              )}

              <div className="auth__field">
                <label className="auth__label">Пароль</label>
                <input
                  type="password"
                  className="auth__input"
                  placeholder="Минимум 6 символов"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
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
                  required
                />
              </div>

              <button
                type="submit"
                className="auth__btn auth__btn--primary"
                disabled={isLoading}
              >
                {isLoading ? 'Регистрация...' : 'Зарегистрироваться'}
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
                  value={loginIdentifier}
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
                  required
                />
              </div>

              <button
                type="submit"
                className="auth__btn auth__btn--primary"
                disabled={isLoading}
              >
                {isLoading ? 'Вход...' : 'Войти'}
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
