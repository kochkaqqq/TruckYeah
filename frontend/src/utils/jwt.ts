export interface JwtPayload {
  email?: string;
  phone?: string;
  name?: string;
  id?: string;
  exp?: number;
}

export function parseJwt(token: string): JwtPayload | null {
  try {
    const base64Url = token.split('.')[1];
    if (!base64Url) return null;

    const normalized = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const base64 = normalized.padEnd(Math.ceil(normalized.length / 4) * 4, '=');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );

    const payload = JSON.parse(jsonPayload);

    // Извлекаем данные из JWT claims (стандартные имена .NET)
    return {
      email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
      phone: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/mobilephone'],
      name: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
      id: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
      exp: payload.exp,
    };
  } catch (error) {
    console.error('Ошибка парсинга JWT:', error);
    return null;
  }
}
