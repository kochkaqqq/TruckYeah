import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { LanguageSelect } from './LanguageSelect';
import './Header.css';

export const Header = () => {
  const { t } = useTranslation();

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
          <LanguageSelect className="header__language-select" />
        </div>
      </div>
    </header>
  );
};
