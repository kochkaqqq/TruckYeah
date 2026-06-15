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
}

interface AuthState {
  currentUser: User | null;
  isAuthenticated: boolean;
  user: User | null;
  setCurrentUser: (user: User | null) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  currentUser: null,
  isAuthenticated: false,
  user: null,
  
  setCurrentUser: (user) => set({ 
    currentUser: user,
    isAuthenticated: !!user,
    user: user
  }),
  
  logout: () => {
    localStorage.removeItem('authToken');
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