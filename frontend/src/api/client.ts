import { parseJwt } from '../utils/jwt';

const BASE_URL = 'http://localhost:5001/api';

export enum UserType {
  Business = 0,
  Private = 1,
  Individual = 2,
}

export interface Country {
  id: string;
  name: string | { value: string };
}

export interface RegistrationDto {
  email: string;
  phone: string;
  password: string;
  userType: UserType;
  name: string;
  surname: string;
  middleName?: string;
  countryId: string;
  postcode: string;
  vatId: string;
  companyId?: string;
  avatarLink?: string;
  deviceId: string;
  userAgent: string;
  ipAddress: string;
}

export interface LoginDto {
  email?: string;
  phone?: string;
  password: string;
  deviceId?: string;
  userAgent?: string;
  ipAddress?: string;
}

export interface User {
  id: string;
  email: string;
  phone?: string;
  name?: string;
  surname?: string;
  fullName?: string;
  userType?: UserType | string;
  isProfileCompleted: boolean;
  city?: string;
  company?: string;
}

export interface AuthResponseData {
  jwtToken?: string;
  refreshToken?: string;
  user?: User;
}

// 🔐 Логика refresh token
let isRefreshing = false;
let refreshSubscribers: Array<(token: string) => void> = [];

function onRefreshed(token: string) {
  refreshSubscribers.forEach((callback) => callback(token));
  refreshSubscribers = [];
}

function addRefreshSubscriber(callback: (token: string) => void) {
  refreshSubscribers.push(callback);
}

// Проверяем, истёк ли токен (с запасом 30 секунд)
function isTokenExpired(token: string): boolean {
  try {
    const payload = parseJwt(token);
    if (!payload || !payload.exp) return true;
    // exp в секундах, Date.now() в миллисекундах
    return payload.exp * 1000 < Date.now() + 30000;
  } catch {
    return true;
  }
}

// Запрос на обновление токена
async function refreshTokenRequest(): Promise<string | null> {
  const refreshToken = localStorage.getItem('refreshToken');
  if (!refreshToken) return null;

  try {
    console.log('🔄 Попытка обновления токена...');
    const response = await fetch(`${BASE_URL}/users/refresh`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(refreshToken),
    });

    if (!response.ok) {
      console.error('❌ Refresh token недействителен');
      return null;
    }

    const data = await response.json();
    if (data.jwtToken && data.refreshToken) {
      localStorage.setItem('authToken', data.jwtToken);
      localStorage.setItem('refreshToken', data.refreshToken);
      console.log('✅ Токен успешно обновлён');
      return data.jwtToken;
    }
    return null;
  } catch (error) {
    console.error('Ошибка обновления токена:', error);
    return null;
  }
}

// Принудительный выход
function forceLogout() {
  console.log(' Принудительный выход из системы');
  localStorage.removeItem('authToken');
  localStorage.removeItem('refreshToken');
  // Редирект на страницу входа
  if (window.location.pathname !== '/auth') {
    window.location.href = '/auth';
  }
}

async function request<T>(url: string, options: RequestInit = {}): Promise<T> {
  let token = localStorage.getItem('authToken');

  // Проверяем истечение токена
  if (token && isTokenExpired(token)) {
    if (isRefreshing) {
      // Другой запрос уже обновляет токен — ждём
      console.log(' Ожидание обновления токена...');
      const newToken = await new Promise<string>((resolve) => {
        addRefreshSubscriber(resolve);
      });
      token = newToken;
    } else {
      isRefreshing = true;
      try {
        const newToken = await refreshTokenRequest();
        if (!newToken) {
          forceLogout();
          throw new Error('Сессия истекла. Пожалуйста, войдите снова.');
        }
        token = newToken;
        onRefreshed(newToken);
      } finally {
        isRefreshing = false;
      }
    }
  }

  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...((options.headers as Record<string, string>) || {}),
  };

  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const response = await fetch(`${BASE_URL}${url}`, {
    ...options,
    headers,
  });

  if (!response.ok) {
    const errorText = await response.text();
    let errorMessage = `Ошибка ${response.status}`;

    try {
      const errorData = JSON.parse(errorText);
      console.error('Детали ошибки сервера:', errorData);
      if (errorData.message) errorMessage = errorData.message;
      if (errorData.title) errorMessage = errorData.title;
      if (errorData.details) errorMessage = `${errorMessage}: ${errorData.details}`;
      if (errorData.errors) {
        const errors = Object.values(errorData.errors).flat().join(', ');
        errorMessage = `${errorMessage}: ${errors}`;
      }
    } catch {
      if (errorText) errorMessage = errorText;
    }

    // Если 401 — возможно токен всё-таки истёк, пробуем refresh
    if (response.status === 401 && !isRefreshing) {
      isRefreshing = true;
      try {
        const newToken = await refreshTokenRequest();
        if (newToken) {
          // Повторяем запрос с новым токеном
          headers['Authorization'] = `Bearer ${newToken}`;
          const retryResponse = await fetch(`${BASE_URL}${url}`, {
            ...options,
            headers,
          });
          if (retryResponse.ok) {
            onRefreshed(newToken);
            const text = await retryResponse.text();
            if (!text) return null as T;
            return JSON.parse(text) as T;
          }
        }
        forceLogout();
        throw new Error('Сессия истекла. Пожалуйста, войдите снова.');
      } finally {
        isRefreshing = false;
      }
    }

    throw new Error(errorMessage);
  }

  if (response.status === 204) return null as T;

  const text = await response.text();
  if (!text) return null as T;

  return JSON.parse(text) as T;
}

export const api = {
  users: {
    register: (data: RegistrationDto) =>
      request<AuthResponseData>('/users/register', {
        method: 'POST',
        body: JSON.stringify(data),
      }),

    login: (data: LoginDto) =>
      request<AuthResponseData>('/users/login', {
        method: 'POST',
        body: JSON.stringify(data),
      }),

    getCurrent: (id: string) =>
      request<User>(`/users/${id}`, {
        method: 'GET',
      }),
  },

  countries: {
    getAll: () =>
      request<Country[]>('/countries/all', {
        method: 'GET',
      }),

    add: (name: string) =>
      request<Country>('/countries', {
        method: 'POST',
        body: JSON.stringify(name),
      }),
  },
};