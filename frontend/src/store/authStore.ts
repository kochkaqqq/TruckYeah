import { create } from 'zustand';

export interface User {
  id: string;
  email: string;
  phone: string;
  fullName?: string;
  userType?: 'client' | 'carrier';
  company?: string;
  isProfileCompleted: boolean;
}

interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  login: (email: string, phone: string) => void;
  register: (email: string, phone: string, password: string) => void;
  setCurrentUser: (user: User) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  isAuthenticated: false,
  
  login: (email, phone) => {
    const user: User = {
      id: '1',
      email,
      phone,
      isProfileCompleted: false,
    };
    set({ user, isAuthenticated: true });
  },
  
  register: (email, phone, _password) => {
    const user: User = {
      id: '1',
      email,
      phone,
      isProfileCompleted: false,
    };
    set({ user, isAuthenticated: true });
  },
  
  setCurrentUser: (user) => set({ user, isAuthenticated: true }),
  
  logout: () => set({ user: null, isAuthenticated: false }),
}));

// Вспомогательные функции
export const getCurrentUser = (): User | null => {
  return useAuthStore.getState().user;
};

export const isAuthenticated = (): boolean => {
  return useAuthStore.getState().isAuthenticated;
};

export const loginUser = (email: string, phone: string) => {
  useAuthStore.getState().login(email, phone);
};

export const registerUser = (email: string, phone: string, password: string) => {
  useAuthStore.getState().register(email, phone, password);
};

export const logoutUser = () => {
  useAuthStore.getState().logout();
};