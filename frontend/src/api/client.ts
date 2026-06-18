import { parseJwt } from '../utils/jwt';
import type { GeoJsonObject } from 'geojson';

const API_BASE_URL = '';

export enum UserType {
  Business = 0,
  Private = 1,
  Individual = 2,
}

export enum ContextType {
  Cargo = 'Cargo',
  Truck = 'Truck',
}

export enum LoadingType {
  Rear = 'Rear',
  Side = 'Side',
  Top = 'Top',
  FullAccess = 'FullAccess',
}

export enum PaymentType {
  Cash = 'Cash',
  WithVAT = 'WithVAT',
  WithoutVAT = 'WithoutVAT',
}

export enum ListingStatus {
  Draft = 'Draft',
  Published = 'Published',
  Archived = 'Archived',
  Completed = 'Completed',
}

export enum ListingVisibility {
  Exchange = 'Exchange',
  Private = 'Private',
}

export enum BidStatus {
  Active = 'Active',
  Accepted = 'Accepted',
  Rejected = 'Rejected',
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
  companyName?: string;
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
  postcode?: string;
  countryId?: string;
  country?: string;
  avatarLink?: string;
  rating?: number;
}

export interface UpdateUserProfileDto {
  email: string;
  phone: string;
  postcode: string;
  name?: string;
  surname?: string;
  middleName?: string;
  city?: string;
  companyName?: string;
  avatarLink?: string;
}

export interface AuthResponseData {
  jwtToken?: string;
  refreshToken?: string;
  user?: User;
}

// ====== Типы для чатов ======
export interface CreateChatDto {
  contextType: ContextType;
  contextId: string;
  recipientUserId: string;
}

export interface Chat {
  id: string;
  contextType: ContextType;
  contextId: string;
  otherParticipantUserId: string;
  lastMessageText: string;
  lastMessageAt: string;
  unreadCount: number;
}

export interface Message {
  id: string;
  chatId: string;
  senderUserId: string;
  text: string;
  createdAt: string;
  readAt?: string;
  isDeleted: boolean;
}

export interface SendMessageDto {
  text: string;
}

export interface UnreadCountDto {
  unreadCount: number;
}

// ====== Типы для грузов ======
export interface RoutePointRequest {
  address?: string;
  lat?: number;
  lon?: number;
  scheduledTime?: string;
  order: number;
}

export interface RoutePointResponse {
  id?: string;
  address?: string;
  lat: number;
  lon: number;
  scheduledTime?: string;
  order: number;
}

export interface RouteCalculation {
  distanceKm: number;
  durationMinutes: number;
  fuelConsumptionLiters: number;
  resolvedPoints: RoutePointResponse[];
  geometry: {
    polyline?: string | null;
    geoJson?: GeoJsonObject | null;
  };
  warnings: string[];
}

export interface CalculateRouteRequest {
  points: RoutePointRequest[];
  fuelConsumptionLitersPer100Km?: number;
  avoidTollRoads?: boolean;
  avoidFerries?: boolean;
}

export interface CreateCargoRequest {
  title?: string;
  cargoName?: string;
  routeFrom?: string;
  routeTo?: string;
  routePoints?: RoutePointRequest[] | null;
  routeDistanceKm?: number;
  routeDurationMinutes?: number;
  routeFuelLiters?: number;
  routeGeometryGeoJson?: string;
  routeCalculatedAt?: string;
  loadDateTime: string;
  unloadDateTime: string;
  weightTons: number;
  volumeM3: number;
  useAutomaticCalculation?: boolean;
  weightPerPackageKg?: number;
  bodyTypeRequired?: string;
  loadingType: LoadingType;
  lengthCm?: number;
  widthCm?: number;
  heightCm?: number;
  palletsCount?: number;
  packagingType?: string;
  requiresCMR?: boolean;
  requiresTIR?: boolean;
  isADR?: boolean;
  requiresTwoDrivers?: boolean;
  paymentType: PaymentType;
  allowBargaining?: boolean;
  prepaymentPercent?: number;
  startingPrice?: number;
  biddingEnabled?: boolean;
  minBidStep?: number;
  visibility?: ListingVisibility;
  boostToTop?: boolean;
  isTemplate?: boolean;
  templateName?: string;
  sourceListingId?: string;
  notes?: string;
}

export interface CargoResponse extends CreateCargoRequest {
  id: string;
  userId: string;
  routePoints?: RoutePointResponse[] | null;
  acceptedBidId?: string | null;
  biddingClosedAt?: string | null;
  status: ListingStatus;
  createdAt: string;
  publishedAt?: string | null;
  boostedUntil?: string | null;
}

export interface CargoSearchParams {
  RouteFrom?: string;
  RouteTo?: string;
  LoadDate?: string;
  WeightFrom?: number;
  WeightTo?: number;
  VolumeFrom?: number;
  VolumeTo?: number;
  BodyType?: string;
  CargoName?: string;
  LoadingType?: LoadingType;
  OnlyWithBidding?: boolean;
  Visibility?: ListingVisibility;
  Page?: number;
  PageSize?: number;
  SortBy?: string;
  SortDirection?: string;
}

export interface CreateCargoBidRequest {
  price: number;
}

export interface CargoBidResponse {
  id: string;
  cargoId: string;
  carrierUserId: string;
  price: number;
  status: BidStatus;
  createdAt: string;
  acceptedAt?: string | null;
}

// ====== Типы для машин ======
export interface CreateTruckRequest {
  title?: string;
  description?: string;
  routeFrom?: string;
  routeTo?: string;
  routePoints?: RoutePointRequest[] | null;
  routeDistanceKm?: number;
  routeDurationMinutes?: number;
  routeFuelLiters?: number;
  routeGeometryGeoJson?: string;
  routeCalculatedAt?: string;
  capacityTons: number;
  volumeM3: number;
  bodyType?: string;
  loadingType: LoadingType;
  crewDriversCount?: number;
  additionalEquipment?: string;
  availableFrom: string;
  price: number;
  paymentType: PaymentType;
  allowBargaining?: boolean;
  prepaymentPercent?: number;
  visibility?: ListingVisibility;
}

export interface TruckResponse extends CreateTruckRequest {
  id: string;
  userId: string;
  routePoints?: RoutePointResponse[] | null;
  status: ListingStatus;
  createdAt: string;
  publishedAt?: string | null;
  sourceListingId?: string | null;
}

export interface TruckSearchParams {
  RouteFrom?: string;
  RouteTo?: string;
  AvailableDate?: string;
  CapacityFrom?: number;
  CapacityTo?: number;
  VolumeFrom?: number;
  VolumeTo?: number;
  BodyType?: string;
  LoadingType?: LoadingType;
  AdditionalEquipment?: string;
  Page?: number;
  PageSize?: number;
  SortBy?: string;
  SortDirection?: string;
}

// ====== Логика refresh token ======
let isRefreshing = false;
let refreshSubscribers: Array<(token: string) => void> = [];

function onRefreshed(token: string) {
  refreshSubscribers.forEach((callback) => callback(token));
  refreshSubscribers = [];
}

function addRefreshSubscriber(callback: (token: string) => void) {
  refreshSubscribers.push(callback);
}

function isTokenExpired(token: string): boolean {
  try {
    const payload = parseJwt(token);
    if (!payload || !payload.exp) return true;
    return payload.exp * 1000 < Date.now() + 30000;
  } catch {
    return true;
  }
}

async function refreshTokenRequest(): Promise<string | null> {
  const refreshToken = localStorage.getItem('refreshToken');
  if (!refreshToken) return null;

  try {
    console.log('🔄 Попытка обновления токена...');
    const response = await fetch('/api/users/refresh', {
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

function forceLogout() {
  console.log('🚪 Принудительный выход из системы');
  localStorage.removeItem('authToken');
  localStorage.removeItem('refreshToken');
  if (window.location.pathname !== '/auth') {
    window.location.href = '/auth';
  }
}

async function request<T>(
  url: string,
  options: RequestInit = {},
  baseUrl: string = API_BASE_URL
): Promise<T> {
  let token = localStorage.getItem('authToken');

  if (token && isTokenExpired(token)) {
    if (isRefreshing) {
      console.log('⏳ Ожидание обновления токена...');
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

  console.log(`🌐 ${options.method || 'GET'} ${baseUrl}${url}`);

  const response = await fetch(`${baseUrl}${url}`, {
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

    if (response.status === 401 && !isRefreshing) {
      isRefreshing = true;
      try {
        const newToken = await refreshTokenRequest();
        if (newToken) {
          headers['Authorization'] = `Bearer ${newToken}`;
          const retryResponse = await fetch(`${baseUrl}${url}`, {
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

// ====== Маппинг ContextType в число для ChatService ======
const contextTypeToNumber: Record<ContextType, number> = {
  [ContextType.Cargo]: 0,
  [ContextType.Truck]: 1,
};

export const api = {
  routes: {
    calculate: (data: CalculateRouteRequest) =>
      request<RouteCalculation>('/Routes/calculate', {
        method: 'POST',
        body: JSON.stringify(data),
      }),
  },

  users: {
    register: (data: RegistrationDto) =>
      request<AuthResponseData>('/api/users/register', {
        method: 'POST',
        body: JSON.stringify(data),
      }),

    login: (data: LoginDto) =>
      request<AuthResponseData>('/api/users/login', {
        method: 'POST',
        body: JSON.stringify(data),
      }),

    getPublic: (id: string) =>
      request<User>(`/api/users/${id}`, {
        method: 'GET',
      }),

    getCurrent: (id: string) =>
      request<User>(`/api/users/${id}`, {
        method: 'GET',
      }),

    getMe: () =>
      request<User>('/api/users/me', {
        method: 'GET',
      }),

    updateMe: (data: UpdateUserProfileDto) =>
      request<User>('/api/users/me', {
        method: 'PUT',
        body: JSON.stringify(data),
      }),
  },

  countries: {
    getAll: () =>
      request<Country[]>('/api/countries/all', {
        method: 'GET',
      }),

    add: (name: string) =>
      request<Country>('/api/countries', {
        method: 'POST',
        body: JSON.stringify(name),
      }),
  },

  // 💬 ChatService API (порт 5003)
  chats: {
    getAll: () =>
      request<Chat[]>('/Chats', {
        method: 'GET',
      }, API_BASE_URL),

    create: (data: CreateChatDto) => {
      const payload = {
        contextType: contextTypeToNumber[data.contextType] ?? 0,
        contextId: data.contextId,
        recipientUserId: data.recipientUserId,
      };

      console.log('💬 Создание чата (payload):', payload);

      return request<Chat>('/Chats', {
        method: 'POST',
        body: JSON.stringify(payload),
      }, API_BASE_URL);
    },

    getMessages: (chatId: string, page = 1, pageSize = 50) =>
      request<Message[]>(`/Chats/${chatId}/messages?page=${page}&pageSize=${pageSize}`, {
        method: 'GET',
      }, API_BASE_URL),

    sendMessage: (chatId: string, data: SendMessageDto) =>
      request<Message>(`/Chats/${chatId}/messages`, {
        method: 'POST',
        body: JSON.stringify(data),
      }, API_BASE_URL),

    markAsRead: (chatId: string) =>
      request<void>(`/Chats/${chatId}/read`, {
        method: 'POST',
      }, API_BASE_URL),

    getUnreadCount: () =>
      request<UnreadCountDto>('/Chats/unread-count', {
        method: 'GET',
      }, API_BASE_URL),
  },

  // 📦 ListingService API (порт 5002) — Грузы
  cargos: {
    search: (params: CargoSearchParams = {}) => {
      const queryParams = new URLSearchParams();
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          queryParams.append(key, value.toString());
        }
      });
      const queryString = queryParams.toString();
      return request<CargoResponse[]>(
        `/Cargos${queryString ? '?' + queryString : ''}`,
        { method: 'GET' },
        API_BASE_URL
      );
    },

    getById: (id: string) =>
      request<CargoResponse>(`/Cargos/${id}`, { method: 'GET' }, API_BASE_URL),

    create: (data: CreateCargoRequest) => {
      console.log('📦 cargos.create - Отправка на сервер:', data);
      return request<string>('/Cargos', {
        method: 'POST',
        body: JSON.stringify(data),
      }, API_BASE_URL);
    },

    update: (id: string, data: CreateCargoRequest) =>
      request<string>(`/Cargos/${id}`, {
        method: 'PUT',
        body: JSON.stringify(data),
      }, API_BASE_URL),

    delete: (id: string) =>
      request<string>(`/Cargos/${id}`, { method: 'DELETE' }, API_BASE_URL),

    getMyCargos: () =>
      request<CargoResponse[]>('/Cargos/my', { method: 'GET' }, API_BASE_URL),

    publish: (id: string) =>
      request<string>(`/Cargos/${id}/publish`, { method: 'POST' }, API_BASE_URL),

    archive: (id: string) =>
      request<string>(`/Cargos/${id}/archive`, { method: 'POST' }, API_BASE_URL),

    copy: (id: string) =>
      request<string>(`/Cargos/${id}/copy`, { method: 'POST' }, API_BASE_URL),

    getBids: (cargoId: string) =>
      request<CargoBidResponse[]>(`/Cargos/${cargoId}/bids`, { method: 'GET' }, API_BASE_URL),

    getMyBids: () =>
      request<CargoBidResponse[]>('/Cargos/bids/my', { method: 'GET' }, API_BASE_URL),

    createBid: (cargoId: string, data: CreateCargoBidRequest) =>
      request<string>(`/Cargos/${cargoId}/bids`, {
        method: 'POST',
        body: JSON.stringify(data),
      }, API_BASE_URL),

    acceptBid: (cargoId: string, bidId: string) =>
      request<string>(`/Cargos/${cargoId}/bids/${bidId}/accept`, { method: 'POST' }, API_BASE_URL),
  },

  // 🚛 ListingService API (порт 5002) — Машины
  trucks: {
    search: (params: TruckSearchParams = {}) => {
      const queryParams = new URLSearchParams();
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          queryParams.append(key, value.toString());
        }
      });
      const queryString = queryParams.toString();
      return request<TruckResponse[]>(
        `/Trucks${queryString ? '?' + queryString : ''}`,
        { method: 'GET' },
        API_BASE_URL
      );
    },

    getById: (id: string) =>
      request<TruckResponse>(`/Trucks/${id}`, { method: 'GET' }, API_BASE_URL),

    create: (data: CreateTruckRequest) => {
      console.log('🚛 trucks.create - Отправка на сервер:', data);
      return request<string>('/Trucks', {
        method: 'POST',
        body: JSON.stringify(data),
      }, API_BASE_URL);
    },

    update: (id: string, data: CreateTruckRequest) =>
      request<string>(`/Trucks/${id}`, {
        method: 'PUT',
        body: JSON.stringify(data),
      }, API_BASE_URL),

    delete: (id: string) =>
      request<string>(`/Trucks/${id}`, { method: 'DELETE' }, API_BASE_URL),

    getMyTrucks: () =>
      request<TruckResponse[]>('/Trucks/my', { method: 'GET' }, API_BASE_URL),

    publish: (id: string) =>
      request<string>(`/Trucks/${id}/publish`, { method: 'POST' }, API_BASE_URL),

    archive: (id: string) =>
      request<string>(`/Trucks/${id}/archive`, { method: 'POST' }, API_BASE_URL),

    copy: (id: string) =>
      request<string>(`/Trucks/${id}/copy`, { method: 'POST' }, API_BASE_URL),
  },
};
