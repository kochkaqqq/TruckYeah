import { useTranslation } from 'react-i18next';

const languages = [
  ['ru', '🇷🇺 Русский'],
  ['en', '🇬🇧 English'],
  ['de', '🇩🇪 Deutsch'],
  ['cs', '🇨🇿 Čeština'],
  ['da', '🇩🇰 Dansk'],
  ['el', '🇬🇷 Ελληνικά'],
] as const;

export const LanguageSelect = ({ className = '' }: { className?: string }) => {
  const { i18n } = useTranslation();
  const selected = (i18n.resolvedLanguage || i18n.language || 'ru').split('-')[0];
  return (
    <select
      className={className}
      value={selected}
      onChange={(event) => void i18n.changeLanguage(event.target.value)}
      aria-label="Язык интерфейса"
    >
      {languages.map(([code, name]) => <option key={code} value={code}>{name}</option>)}
    </select>
  );
};
