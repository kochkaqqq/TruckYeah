import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import './Header.css';

export const Header = () => {
  const { t, i18n } = useTranslation();  // ← Обратите внимание: i18n здесь

  const languages = [
    { code: 'ru', name: '🇷🇺 Русский' },
    { code: 'en', name: '🇬🇧 English' },
    { code: 'de', name: '🇩🇪 Deutsch' },
    { code: 'fr', name: '🇫🇷 Français' },
    { code: 'es', name: '🇪🇸 Español' },
    { code: 'it', name: '🇮🇹 Italiano' },
    { code: 'pt', name: '🇵🇹 Português' },
    { code: 'nl', name: '🇳🇱 Nederlands' },
    { code: 'pl', name: '🇵🇱 Polski' },
    { code: 'sv', name: '🇸🇪 Svenska' },
    { code: 'da', name: '🇩🇰 Dansk' },
    { code: 'fi', name: '🇫🇮 Suomi' },
    { code: 'no', name: '🇳🇴 Norsk' },
    { code: 'el', name: '🇬🇷 Ελληνικά' },
    { code: 'cs', name: '🇨 Čeština' },
  ];

  return (
    <header className="header">
      <div className="header__container">
        <Link to="/" className="header__logo">
          {t('header.logo')}
        </Link>
        
        <nav className="header__nav">
          <Link to="/cargo" className="header__link">{t('header.cargos')}</Link>
          <Link to="/vehicles" className="header__link">{t('header.vehicles')}</Link>
          <Link to="/chats" className="header__link">{t('header.chats')}</Link>
          <Link to="/trades" className="header__link">{t('header.trades')}</Link>
          <Link to="/my-listing" className="header__link">{t('header.myListings')}</Link>
          <Link to="/profile" className="header__link">{t('header.profile')}</Link>
        </nav>

        <div className="header__actions">
          <select 
            className="header__language-select"
            value={i18n.language}  // ← i18n.language
            onChange={(e) => i18n.changeLanguage(e.target.value)}  // ← i18n.changeLanguage
          >
            {languages.map(lang => (
              <option key={lang.code} value={lang.code}>
                {lang.name}
              </option>
            ))}
          </select>
        </div>
      </div>
    </header>
  );
};