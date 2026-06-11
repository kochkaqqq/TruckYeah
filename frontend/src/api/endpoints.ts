export const ENDPOINTS = {
  auth: {
    login: '/api/users/login',
    register: '/api/users/register',
    byId: (id: string) => `/api/users/${id}`,
  },
  orders: {
    list: '/Cargos',
    byId: (id: string) => `/Cargos/${id}`,
    create: '/Cargos',
    update: (id: string) => `/Cargos/${id}`,
  },
  carriers: {
    list: '/Trucks',
    byId: (id: string) => `/Trucks/${id}`,
  },
} as const;
