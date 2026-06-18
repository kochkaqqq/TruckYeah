import type { CargoResponse, TruckResponse, UserType } from './client';

const MODERATOR_TOKEN_KEY = 'moderatorAuthToken';
const MODERATOR_REFRESH_KEY = 'moderatorRefreshToken';

export type AccountRole = 'User' | 'Moderator';
export type AccountStatus = 'Active' | 'Blocked';

export interface ModeratorLoginRequest {
  email: string;
  password: string;
  deviceId?: string;
  userAgent?: string;
  ipAddress?: string;
}

export interface ModeratorLoginResponse {
  jwtToken: string;
  refreshToken: string;
}

export interface AdminUser {
  id: string;
  email: string;
  phone: string;
  displayName: string;
  userType: UserType | string;
  role: AccountRole;
  status: AccountStatus;
  city?: string;
  company?: string;
  avatarLink?: string;
  rating: number;
  createdAt: string;
}

interface JwtPayload {
  role?: string;
  exp?: number;
  [key: string]: unknown;
}

function readJwtPayload(token: string): JwtPayload | null {
  try {
    const part = token.split('.')[1];
    const normalized = part.replace(/-/g, '+').replace(/_/g, '/');
    const decoded = decodeURIComponent(
      atob(normalized)
        .split('')
        .map((char) => `%${char.charCodeAt(0).toString(16).padStart(2, '0')}`)
        .join(''),
    );
    return JSON.parse(decoded) as JwtPayload;
  } catch {
    return null;
  }
}

export function isModeratorToken(token: string | null): boolean {
  if (!token) return false;
  const payload = readJwtPayload(token);
  const role =
    payload?.role ??
    payload?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
  return role === 'Moderator' && (!payload?.exp || payload.exp * 1000 > Date.now());
}

export function getModeratorToken(): string | null {
  return localStorage.getItem(MODERATOR_TOKEN_KEY);
}

export function clearModeratorSession(): void {
  localStorage.removeItem(MODERATOR_TOKEN_KEY);
  localStorage.removeItem(MODERATOR_REFRESH_KEY);
}

async function moderatorRequest<T>(
  path: string,
  options: RequestInit = {},
  authenticated = true,
): Promise<T> {
  const headers = new Headers(options.headers);
  headers.set('Content-Type', 'application/json');

  if (authenticated) {
    const token = getModeratorToken();
    if (!token) throw new Error('Сессия модератора не найдена.');
    headers.set('Authorization', `Bearer ${token}`);
  }

  const response = await fetch(`/api/moderator${path}`, { ...options, headers });
  if (response.status === 401 || response.status === 403) {
    if (authenticated) clearModeratorSession();
    throw new Error(
      response.status === 403
        ? 'У этой учётной записи нет прав модератора.'
        : 'Сессия истекла. Войдите снова.',
    );
  }

  if (!response.ok) {
    const raw = await response.text();
    let data: { detail?: string; title?: string } | undefined;
    try {
      data = JSON.parse(raw) as { detail?: string; title?: string };
    } catch {
      data = undefined;
    }
    throw new Error(data?.detail || data?.title || raw || `Ошибка ${response.status}`);
  }

  if (response.status === 204) return undefined as T;
  const text = await response.text();
  return text ? (JSON.parse(text) as T) : (undefined as T);
}

export const moderatorApi = {
  auth: {
    async login(data: ModeratorLoginRequest): Promise<ModeratorLoginResponse> {
      const response = await moderatorRequest<ModeratorLoginResponse>(
        '/login',
        {
          method: 'POST',
          body: JSON.stringify({
            ...data,
            deviceId: data.deviceId || 'moderator-web',
            userAgent: data.userAgent || navigator.userAgent,
            ipAddress: data.ipAddress || 'web-client',
          }),
        },
        false,
      );

      if (!isModeratorToken(response.jwtToken)) {
        throw new Error('У этой учётной записи нет прав модератора.');
      }

      localStorage.setItem(MODERATOR_TOKEN_KEY, response.jwtToken);
      localStorage.setItem(MODERATOR_REFRESH_KEY, response.refreshToken);
      return response;
    },
    async logout(): Promise<void> {
      const refreshToken = localStorage.getItem(MODERATOR_REFRESH_KEY);
      try {
        if (refreshToken && getModeratorToken()) {
          await moderatorRequest<void>('/logout', {
            method: 'POST',
            body: JSON.stringify(refreshToken),
          });
        }
      } finally {
        clearModeratorSession();
      }
    },
  },
  users: {
    getAll: () => moderatorRequest<AdminUser[]>('/users'),
    block: (id: string) =>
      moderatorRequest<void>(`/users/${id}/block`, { method: 'POST' }),
    unblock: (id: string) =>
      moderatorRequest<void>(`/users/${id}/unblock`, { method: 'POST' }),
  },
  cargos: {
    getAll: () => moderatorRequest<CargoResponse[]>('/cargos'),
    approve: (id: string) =>
      moderatorRequest<string>(`/cargos/${id}/approve`, { method: 'POST' }),
    reject: (id: string, reason: string) =>
      moderatorRequest<string>(`/cargos/${id}/reject`, {
        method: 'POST',
        body: JSON.stringify({ reason }),
      }),
  },
  trucks: {
    getAll: () => moderatorRequest<TruckResponse[]>('/trucks'),
    approve: (id: string) =>
      moderatorRequest<string>(`/trucks/${id}/approve`, { method: 'POST' }),
    reject: (id: string, reason: string) =>
      moderatorRequest<string>(`/trucks/${id}/reject`, {
        method: 'POST',
        body: JSON.stringify({ reason }),
      }),
  },
};
