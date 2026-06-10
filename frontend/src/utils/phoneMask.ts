// Форматирование телефона в формат +7 (XXX) XXX-XX-XX
export const formatPhone = (value: string): string => {
  // Удаляем все нецифровые символы
  const digits = value.replace(/\D/g, '');
  
  // Если начинается с 8, заменяем на 7
  if (digits.startsWith('8')) {
    return formatPhone('7' + digits.slice(1));
  }
  
  // Если не начинается с 7, добавляем 7
  if (!digits.startsWith('7') && digits.length > 0) {
    return formatPhone('7' + digits);
  }
  
  // Форматируем: +7 (XXX) XXX-XX-XX
  if (digits.length === 0) return '';
  if (digits.length === 1) return `+${digits[0]}`;
  if (digits.length <= 4) return `+${digits[0]} (${digits.slice(1)}`;
  if (digits.length <= 7) return `+${digits[0]} (${digits.slice(1, 4)}) ${digits.slice(4)}`;
  if (digits.length <= 9) return `+${digits[0]} (${digits.slice(1, 4)}) ${digits.slice(4, 7)}-${digits.slice(7)}`;
  return `+${digits[0]} (${digits.slice(1, 4)}) ${digits.slice(4, 7)}-${digits.slice(7, 9)}-${digits.slice(9, 11)}`;
};

// Получаем только цифры из отформатированного телефона
export const getDigitsFromPhone = (formattedPhone: string): string => {
  return formattedPhone.replace(/\D/g, '');
};