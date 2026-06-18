import { create } from 'zustand';
import type { UserType } from '../api/client';

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
  postcode?: string;
  countryId?: string;
  country?: string;
  avatarLink?: string;
  rating?: number;
}

interface AuthState {
  currentUser: User | null;
  isAuthenticated: boolean;
  isInitialized: boolean;
  user: User | null;
  setCurrentUser: (user: User | null) => void;
  updateUser: (data: Partial<User>) => void;
  login: (user: User) => void;
  markInitialized: () => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  currentUser: null,
  isAuthenticated: !!localStorage.getItem('authToken'),
  isInitialized: false,
  user: null,

  setCurrentUser: (user) =>
    set({
      currentUser: user,
      isAuthenticated: !!user,
      isInitialized: true,
      user,
    }),

  updateUser: (data) =>
    set((state) => {
      const updatedUser = state.user ? { ...state.user, ...data } : null;
      return {
        currentUser: updatedUser,
        user: updatedUser,
      };
    }),

  login: (user) =>
    set({
      currentUser: user,
      isAuthenticated: true,
      isInitialized: true,
      user,
    }),

  markInitialized: () => set({ isInitialized: true }),

  logout: () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('refreshToken');
    set({
      currentUser: null,
      isAuthenticated: false,
      isInitialized: true,
      user: null,
    });
  },
}));

export const getCurrentUser = (): User | null =>
  useAuthStore.getState().currentUser;
