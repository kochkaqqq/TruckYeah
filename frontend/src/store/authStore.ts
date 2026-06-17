import { create } from 'zustand';
import { UserType } from '../api/client';

export interface User {
  id: string;
  email: string;
  phone?: string;
  name?: string;
  surname?: string;
  fullName?: string;
  userType?: UserType | 'client' | 'carrier' | string;
  isProfileCompleted: boolean;
  city?: string;
  company?: string;
}

interface AuthState {
  currentUser: User | null;
  isAuthenticated: boolean;
  user: User | null;
  setCurrentUser: (user: User | null) => void;
  updateUser: (data: Partial<User>) => void;
  login: (user: User) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  // ✅ Инициализация из localStorage
  currentUser: null,
  isAuthenticated: !!localStorage.getItem('authToken'), // ✅ Ключевое исправление
  user: null,
  
  setCurrentUser: (user) => set({ 
    currentUser: user,
    isAuthenticated: !!user,
    user: user
  }),

  updateUser: (data) => set((state) => {
    const updatedUser = state.user ? { ...state.user, ...data } : null;
    return {
      currentUser: updatedUser,
      user: updatedUser,
    };
  }),

  // ✅ Добавляем метод login
  login: (user) => {
    set({
      currentUser: user,
      isAuthenticated: true,
      user: user,
    });
  },
  
  logout: () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('refreshToken');
    set({ 
      currentUser: null,
      isAuthenticated: false,
      user: null
    });
  },
}));

export const getCurrentUser = (): User | null => {
  return useAuthStore.getState().currentUser;
};