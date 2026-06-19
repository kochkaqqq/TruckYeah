import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import LanguageDetector from 'i18next-browser-languagedetector';

import ru from './locales/ru.json';
import en from './locales/en.json';
import de from './locales/de.json';
import da from './locales/da.json';
import el from './locales/el.json';
import cs from './locales/cs.json';

const resources = {
  ru: { translation: ru },
  en: { translation: en },
  de: { translation: de },
  da: { translation: da },
  el: { translation: el },
  cs: { translation: cs },
};

export const supportedLanguages = Object.keys(resources);

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources,
    supportedLngs: supportedLanguages,
    fallbackLng: 'ru',
    debug: false,
    interpolation: {
      escapeValue: false,
    },
    detection: {
      order: ['localStorage', 'navigator'],
      caches: ['localStorage'],
    },
  });

export default i18n;
