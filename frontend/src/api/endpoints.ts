export const ENDPOINTS = {
  auth: {
    login: '/api/auth/login',
    register: '/api/auth/register',
    me: '/api/auth/me',
  },
  orders: {
    list: '/api/orders',
    byId: (id: string) => `/api/orders/${id}`,
    create: '/api/orders',
    update: (id: string) => `/api/orders/${id}`,
  },
  carriers: {
    list: '/api/carriers',
    byId: (id: string) => `/api/carriers/${id}`,
  },
} as const;
