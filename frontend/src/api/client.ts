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
}

export interface AuthResponseData {
  jwtToken?: string;
  refreshToken?: string;
  user?: User;
}

async function request<T>(
  url: string,
  options: RequestInit = {}
): Promise<T> {
  const response = await fetch(`${BASE_URL}${url}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options.headers,
    },
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

    getCurrent: () =>
      request<User>('/users/me', {
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