import { create } from 'zustand';
import {
  clearModeratorSession,
  getModeratorToken,
  isModeratorToken,
  moderatorApi,
} from '../api/moderatorClient';

interface ModeratorState {
  isAuthenticated: boolean;
  isBusy: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
}

export const useModeratorStore = create<ModeratorState>((set) => ({
  isAuthenticated: isModeratorToken(getModeratorToken()),
  isBusy: false,

  login: async (email, password) => {
    set({ isBusy: true });
    try {
      await moderatorApi.auth.login({ email, password });
      set({ isAuthenticated: true });
    } finally {
      set({ isBusy: false });
    }
  },

  logout: async () => {
    set({ isBusy: true });
    try {
      await moderatorApi.auth.logout();
    } finally {
      clearModeratorSession();
      set({ isAuthenticated: false, isBusy: false });
    }
  },
}));
